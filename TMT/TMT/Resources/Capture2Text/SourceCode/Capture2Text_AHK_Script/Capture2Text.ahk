;-------------------------------------------------------------------------
;  Copyright (C) 2010-2014 Christopher Brochtrup
;
;  This file is part of Capture2Text.
;
;  Capture2Text is free software: you can redistribute it and/or modify
;  it under the terms of the GNU General Public License as published by
;  the Free Software Foundation, either version 3 of the License, or
;  (at your option) any later version.
;
;  Capture2Text is distributed in the hope that it will be useful,
;  but WITHOUT ANY WARRANTY; without even the implied warranty of
;  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;  GNU General Public License for more details.
;
;  You should have received a copy of the GNU General Public License
;  along with Capture2Text.  If not, see <http://www.gnu.org/licenses/>.
;
;-------------------------------------------------------------------------

#NoEnv                       ; Avoids checking empty variables to see if they are environment variables 
#KeyHistory 0                ; Disable key history
#SingleInstance force        ; Skips the dialog box and replaces the old instance automatically
SendMode Input               ; Use faster and more reliable send method
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory
CoordMode, Mouse, Screen     ; Make 0,0 the top-left of screen
SetTitleMatchMode 2          ; Allow partial window title matches
FileEncoding, UTF-8          ; Use UTF-8 file encoding

#Include %A_ScriptDir%/ScreenCapture.ahk
#Include %A_ScriptDir%/Common.ahk
#Include %A_ScriptDir%/SettingsDlg.ahk
#Include %A_ScriptDir%/PopupDlg.ahk

LEPTONICA_UTIL      = Utils/leptonica_util/leptonica_util.exe ; OCR pre-processing executable
OCR_UTIL_NHOCR      = Utils/nhocr/nhocr.exe         ; NHocr OCR executable
OCR_UTIL_TESSERACT  = Utils/tesseract/tesseract.exe ; Tesseract OCR executable
SUBSTITUTIONS_FILE  = substitutions.txt
BOX_UPDATE_RATE     = 1    ; Update rate of the capture box (ms)
PREVIEW_UPDATE_RATE = 100  ; Ideal update rate of the preview box (ms). The OCR may take longer to perform though.

captureMode        = 0   ; 1 = In capture mode
topLeftCoordActive = 1   ; 0 = The top-left coordinate is active; 1 = The bottom-right coordinate is active
moveBothCorners    = 0   ; 1 = Move both corners at the same time
bothCornersHeight  = 0   ; Height of the capture box in "move both corners" mode
bothCornersWidth   = 0   ; Width of the capture box in "move both corners" mode
x1                 = 0   ; X1-Coord of the capture box
y1                 = 0   ; Y1-Coord of the capture box
x2                 = 0   ; X2-Coord of the capture box
y2                 = 0   ; Y2-Coord of the capture box
dictionary         = English ; The OCR dictionary to use
utteranceList      := [] ; Voice recognition results returned by Google
voiceCaptureMode   = 0   ; 1 = In voice capture mode
subList            := [] ; Substitutions list

; Create ocrDicTable and ocrCodeTable
createLanguageTable()

; Create installedDics
getInstalledDics()

; Create voiceTable
createVoiceTable()

; Read the settings in settings.ini
readSettings()

; Set the dictionary directory for the OCR tool
EnvSet, NHOCR_DICDIR, Utils/nhocr/Dic

; Read in the substitutions file
readsubstitutions()

numArg = %0% ; Number of command line arguments passed in by the user

commandLineMode := (numArg >= 4) ; 1 = In Command Line Mode

; Handle command line arguments (if provided)
if(commandLineMode)
{
  x1 = %1%
  y1 = %2%
  x2 = %3%
  y2 = %4%
  outFile = %5%

  doCommandLine(x1, y1, x2, y2, outFile)
}

; Configure the tray
configureTray()

; Create the hotkeys based on user preference
createHotKeys()

; Run HandleExit when this program is closed
OnExit, HandleExit

; Show message on the first run after install
showFirstRunMessage()

return

;-------------------------------------------------------------------------

; Process the command line arguments that user has passed in
doCommandLine(x1, y1, x2, y2, outFile="")
{
  global

  ; Remove tray icon (it will still briefly appear though)
  menu, tray, NoIcon

  ; Capture the screen and OCR it
  ocrText := captureAndOCR(x1, y1, x2, y2)

  ; Replace certain characters in the text
  ocrText := replaceOCRText(ocrText)

  ; Output the OCR'd text based on user-defined settings (default is clipboard)
  doOutput(ocrText)

  ; If the user has specified a file to write the text to
  if(StrLen(outFile) > 0)
  {
    f := FileOpen(outFile, "w", 65001)
    f.Write(ocrText)
    f.Close()
  }

  ; If the popup option is not set, close program
  ; Otherwise, program will close when popup is dismissed (see PopupDlg.ahk)
  if(!popupWindow)
  {
    ExitApp
  }

} ; doCommandLine()


; Show message on the first run after install
showFirstRunMessage()
{
  global

  if(firstRun)
  {
    ; Prevent this message from appearing again
    IniWrite, 0, %SETTINGS_FILE%, Misc, FirstRun

    firstRunMsg = Hello, this appears to be your first time running %PROG_NAME% v%VERSION%.`r`n`r`n
    firstRunMsg = %firstRunMsg%%PROG_NAME% is operated through the use of hotkeys (ie. keyboard`r`n
    firstRunMsg = %firstRunMsg%shortcuts). It is recommended that you familiarize yourself with`r`n
    firstRunMsg = %firstRunMsg%the hotkeys and other settings by right-clicking the %PROG_NAME% tray`r`n
    firstRunMsg = %firstRunMsg%icon in the bottom-right corner and selecting the "Preferences..."`r`n
    firstRunMsg = %firstRunMsg%option.`r`n`r`n
    firstRunMsg = %firstRunMsg%Thank you!`r`n`r`n(This message will not appear again)`r`n

    MsgBox, 64, %PROG_NAME%, %firstRunMsg%
  }

} ; showFirstRunMessage


; On first click start the capture box, on second click capture the image in
; the capture box and send to OCR
Capture:
  if(captureMode)
  {
    ; Update the active capture point
    updateActiveCoordWithMousePos()

    ; End capture mode
    endCaptureMode()

    ; Capture the screen and OCR it
    ocrText := captureAndOCR(x1, y1, x2, y2)

    ; Replace certain characters in the text
    ocrText := replaceOCRText(ocrText)

    ; Output the OCR'd text based on user-defined settings (default is clipboard)
    doOutput(ocrText)
  }
  else ; Not in capture mode
  {
    ; Start capture mode
    startCaptureMode()
  }
return ; Capture:


; Capture voice, convert to text via Google, present text to user
CaptureVoice:
  voiceAudioFile   = Output/voice.flac
  SpeechToTextFile = Output/speech_to_text.txt

  ; Specify that voice capture mode has started
  voiceCaptureMode = 1

  ; End OCR capture for the cases were the user interrupted an OCR capture with a voice capture
  endCaptureMode()

  ; Limit number of voice results to 1-9 range
  if(voiceMaxResults > 9)
  {
    voiceMaxResults = 9
  }
  else if(voiceMaxResults < 1)
  {
    voiceMaxResults = 1
  }

  ; Show recording message in top left corner
  Gui, VoiceStatus:Font, s20
  Gui, VoiceStatus:Add, Text, vVoiceStatusText X26 Y18 c00AAFF, Recording...
  Gui, VoiceStatus:Color, 080808
  Gui, VoiceStatus:+LastFound +AlwaysOnTop -Caption +ToolWindow +Border
  WinSet, Transparent, 240
  Gui, VoiceStatus:Show, X0 Y0 W200 H70, Voice Capture Status - Capture2Text

  ; Run SoX to get microphone input from user. SoX is setup to stop when a small
  ; amount of silence is encountered. Google API need FLAC @ 16000 sample rate.
  ; I tried using a noise profile, but Google API seems to trained to prefer some noise.
  RunWait, Utils/sox/sox.exe -d %voiceAudioFile% silence 0`% 2 00:00:%VoiceSilenceBeforeStop% 1.1`% channels 1 rate 16k, , Hide

  ; If the user pressed ESC while recording, stop voice capture mode before sending to Google
  if(!voiceCaptureMode)
  {
    Gui, VoiceStatus:Destroy
    return
  }

  ; Show analyzing message
  GuiControl, VoiceStatus:Text, VoiceStatusText, Analyzing...

  ; Convert language to language code
  voiceLanguageCode := voiceTable[voiceLanguage]

  ; Send the FLAC to Google for processing
  ; maxresults=#: Control the number of results (sorted by confidence)
  ; lang=str: The language to use
  ; pfilter=0: Turn off profanity filter (set to 2 to enable - its also the default)
  RunWait, Utils/sox/wget.exe -q -U "Mozilla/5.0" --post-file %voiceAudioFile% --header="Content-Type: audio/x-flac; rate=16000" -O - "http://www.google.com/speech-api/v1/recognize?lang=%voiceLanguageCode%&maxresults=%voiceMaxResults%&pfilter=0&client=chromium" -O  %SpeechToTextFile%, , Hide

  ; Remove the message
  Gui, VoiceStatus:Destroy

  ; If the user pressed ESC while analyzing, stop voice capture mode before outputting results
  if(!voiceCaptureMode)
  {
    return
  }

  ; Read the JSON returned from Google that contains the recognized text
  file := FileOpen(SpeechToTextFile, "r", 65001)
  fileText := file.ReadLine()
  file.Close()

  ; Parse Google response and store in utteranceList
  parseGoogleVoiceRecognitionResponse(fileText)

  ; If at least one result was returned from Google
  if(utteranceList.MaxIndex() > 0)
  {
    ; If the user specified that they want to see the results list
    if((voiceMaxResults > 1) && (utteranceList.MaxIndex() > 1))
    {
      Gui, VoiceResults:Margin, 6, 6

      ; If the user specified a font, use it
      if(StrLen(voiceResultsWindowFont) > 0)
      {
        fontName = %voiceResultsWindowFont%
      }

      ; Set button font
      Gui, VoiceResults:font, s%voiceResultsWindowFontSize%, %fontName%

      ; Create a button for each result
      Loop % utteranceList.MaxIndex()
      {
        curUtterance := % utteranceList[A_Index]

        ; Create the button text
        buttonText = % " " A_Index ". " curUtterance

        ; Add the button for this result
        Gui, VoiceResults:Add, Button, Left gHandleVoiceResultsSelection vutteranceSelection%A_Index%, %buttonText%
      }

      ; If user specified GUI width, use it (otherwise, GUI will expand to fit buttons)
      if(StrLen(voiceResultsWindowWidth) > 0)
      {
        widthString = W%voiceResultsWindowWidth%
      }

      ; Show the Choose (Voice) Result dialog
      Gui, VoiceResults:-MinimizeBox +Resize +ToolWindow +AlwaysOnTop
      Gui, VoiceResults:Show, X0 Y0 %widthString%, Choose Result - Capture2Text
    }
    else ; User wants to automatically use the first result
    {
      ; Output the recognized text based on user-defined settings (default is clipboard)
      doOutput(selectedText)

      ; End voice capture mode
      voiceCaptureMode = 0
    }
  }
  else ; No results returned from Google
  {
    voiceCaptureMode = 0
  }
return ; CaptureVoice:


; Parse the JSON response from the Google voice recognition service
; and store each utterance into utteranceList.
; Params:
;   string responseText - The JSON returned from Google
parseGoogleVoiceRecognitionResponse(responseText)
{
  global

  foundPos = 0
  utteranceList := []

  ; Parse the JSON and store all utterances (results) into utteranceList
  Loop, %voiceMaxResults%
  {
    foundPos := foundPos + 1
    foundPos := RegExMatch(fileText, """utterance"":""(.*?)""", utterance, foundPos)

    if(foundPos)
    {
      utteranceList.Insert(utterance1)
    }
    else
    {
      break
    }
  }
} ; parseGoogleVoiceRecognitionResponse


; Handle a button click on one of the voice results in the Choose (Voice) Result dialog
HandleVoiceResultsSelection:
  ; Get the text of the control
  GuiControlGet, buttonText,, %A_GuiControl%

  ; Get the result number
  optionSelected := SubStr(buttonText, 2, 1)

  Gui, VoiceResults:Destroy

  ; Get the text for the result
  selectedText := utteranceList[optionSelected]

  ; Output the recognized text based on user-defined settings (default is clipboard)
  doOutput(selectedText)

  ; End voice capture mode
  voiceCaptureMode = 0

return ; HandleVoiceResultsSelection:


; This label is automatically associated with a GUI. It triggers when the X is pressed.
VoiceResultsGuiClose:
  Gui, VoiceResults:Destroy

  ; End voice capture mode
  voiceCaptureMode = 0

return ; GuiClose:


; This label is automatically associated with a GUI. It triggers when the GUI is resized.
VoiceResultsGuiSize:
  ; If in voice capture mode
  if(voiceCaptureMode)
  {
    ; Calculate width of buttons based on GUI width
    NewWidth := A_GuiWidth - 12

    ; Expand width of each button the GUI's width
    Loop % utteranceList.MaxIndex()
    {
      GuiControl, Move, utteranceSelection%A_Index%, W%NewWidth%
    }
  }
return


; When the Voice Capture Status dialog is present, activate these hotkeys:
#IfWinActive, Voice Capture Status - Capture2Text

; When escape key is pressed
Escape::
  ; End voice capture mode
  voiceCaptureMode = 0

  Gui, VoiceResults:Destroy
return ; Escape::

; Done specifying hotkeys for the Voice Capture Status dialog
#IfWinActive


; When the Choose (Voice) Result dialog is present, activate these hotkeys:
#IfWinActive, Choose Result - Capture2Text

; Keys 1-9 will select a result
1::
2::
3::
4::
5::
6::
7::
8::
9::
  ; Get string representation of the hotkey that was pressed (1-9)
  optionSelected := A_ThisHotkey

  ; If the hotkey corresponds to a result in the dialog
  if(optionSelected <= utteranceList.MaxIndex())
  {
    Gui, VoiceResults:Destroy

    ; Get the text for the result
    selectedText := utteranceList[optionSelected]

    ; Output the recognized text based on user-defined settings (default is clipboard)
    doOutput(selectedText)

    ; End voice capture mode
    voiceCaptureMode = 0
  }

return ; 1-9::


; When escape key is pressed
Escape::
  Gui, VoiceResults:Destroy

  ; End voice capture mode
  voiceCaptureMode = 0

return ; Escape::


; When enter key is pressed
Enter::
NumpadEnter::
  Gui, VoiceResults:Destroy

  ; Get the first result
  selectedText := utteranceList[1]

  ; Output the recognized text based on user-defined settings (default is clipboard)
  doOutput(selectedText)

  ; End voice capture mode
  voiceCaptureMode = 0

return ; When enter key is pressed

; Done specifying hotkeys for the Choose (Voice) Result dialog
#IfWinActive


; Toggle the active capture box corner
ToggleActiveCaptureCorner:
  if(topLeftCoordActive)
  {
    topLeftCoordActive = 0
    MouseMove, x2, y2, 0
    updateActiveCoordWithMousePos()
  }
  else
  {
    topLeftCoordActive = 1
    MouseMove, x1, y1, 0
    updateActiveCoordWithMousePos()
  }
return ; ToggleActiveCaptureCorner:


; Start "move both corners" mode
StartMoveCapture:
  bothCornersWidth := x2 - x1
  bothCornersHeight := y2 - y1
  moveBothCorners = 1
return ; StartMoveCapture:


; End "move both corners" mode
EndMoveCapture:
  moveBothCorners = 0
return ; EndMoveCapture:


; Cancel the capture
CancelCapture:
  endCaptureMode()
return ; CancelCapture:


; Nudge the capture to the left
NudgeLeftKey:
  nudgeNonActiveCaptureCoords(-1, 0)
return ; NudgeLeftKey:


; Nudge the capture to the right
NudgeRightKey:
  nudgeNonActiveCaptureCoords(1, 0)
return ; NudgeRightKey:


; Nudge the capture up
NudgeUpKey:
  nudgeNonActiveCaptureCoords(0, -1)
return ; NudgeUpKey:


; Nudge the capture down
NudgeDownKey:
  nudgeNonActiveCaptureCoords(0, 1)
return ; NudgeDownKey:


; Switch to dictionary 1
HandleKeyDictionary1:
  switchToDic(dictionary1)
  displayOCRLanguageInTray(dictionary1)
return ; HandleKeyDictionary1:


; Switch to dictionary 2
HandleKeyDictionary2:
  switchToDic(dictionary2)
  displayOCRLanguageInTray(dictionary2)
return ; HandleKeyDictionary2:


; Switch to dictionary 3
HandleKeyDictionary3:
  switchToDic(dictionary3)
  displayOCRLanguageInTray(dictionary3)
return ; HandleKeyDictionary3:


; Toggle OCR pre-processing
HandleOcrPreprocessingKey:
  toggleOcrPreprocessing()
return ; HandleOcrPreprocessingKey:


HandleKeyTextDirectionToggle:
  toggleTextDirection()
  displayTextDirectionInTray()
return ; HandleKeyTextDirectionToggle


; Toggle voice language
HandleKeyVoiceLanguage:
  toggleVoiceLanguage()
  uncheckVoiceLangMenuItems()
  checkVoiceLangMenuItem(voiceLanguage)
  displayVoiceLanguageInTray()
return ; HandleKeyVoiceLanguage:


; Display the language of the provided dictionary in the tray
displayOCRLanguageInTray(dic)
{
  global
  SetTrayTip(dic, 1000)
} ; displayOCRLanguageInTray


; Display the current text direction in the tray
displayTextDirectionInTray()
{
  global
  textDirectionMsg = %textDirection% text
  SetTrayTip(textDirectionMsg, 1000)

} ; displayTextDirectionInTray


; Display the current voice language in the tray
displayVoiceLanguageInTray()
{
  global
  voiceLangMsg = % "Speech Recognition Language: `n" voiceLanguage
  SetTrayTip(voiceLangMsg, 1000)

} ; displayVoiceLanguageInTray


; Get the size of a text control (in pixels)
; str    - The string displayed in the text control
; size   - The font size of the text
; font   - The font of the text
; height - 1 = return height as well as width, 0 = return only the width
getTextSize(str, size, font, height=false)
{
  global

  ; Creating a temporary hidden window, add the text, get size, delete window
  Gui, TextSizeWindow:Font, s%size%, %font%
  Gui, TextSizeWindow:Add, Text, vsizeText, %str%
  GuiControlGet outSize, TextSizeWindow:Pos, sizeText
  Gui, TextSizeWindow:Destroy

  Return height ? outSizeW "," outSizeH : outSizeW

} ; GetTextSize()


; Update the preview box text
UpdatePreviewBox:
  ; Remove the capture box before screen capture. The OCR will be more accurate
  ; without the capture box partially obscuring the text.
  if(PreviewRemoveCaptureBox)
  {
    Gui, Capture:Destroy
  }

  ; Capture the screen and OCR it
  ocrText := captureAndOCR(x1, y1, x2, y2)

  ; Replace certain characters in the text
  ocrText := replaceOCRText(ocrText, true)

  previewTextStrLen := StrLen(ocrText)

  ; Limit preview to certain number of characters
  if(previewTextStrLen > 150)
  {
    ocrText := SubStr(ocrText, 1, 150)
    ocrText = %ocrText%...
  }

  ; If still in capture mode after the OCR has finished
  if(captureMode)
  {
    ; Get the size of the OCR'd text on screen (in pixels)
    textSize := getTextSize(ocrText, previewBoxFontSize, previewBoxFont)

    ; Resize of the preview box's text control to fit entire text
    GuiControl, Preview:Text, PreviewText, %ocrText%

    ; Resize the preview box itself to fit entire text control
    GuiControl, Preview:Move, PreviewText, W%textSize%

    ; Show the resized preview box with the new text
    if(previewLocation == "Fixed") 
    {
      previewX := 0
      previewY := 0
    }
    else ; Dynamic
    {
      previewX := x2 + 10
      previewY := y1
    }

    Gui, Preview:Show, X%previewX% Y%previewY% AutoSize

    ; Start another one-time preview timer
    SetTimer, UpdatePreviewBox, -%PREVIEW_UPDATE_RATE%
  }

return ; UpdatePreviewBox:


; Update the position of the capture box.
UpdateCaptureBox:
  ; Set the box color
  Gui, Capture:Color, %captureBoxColor%

  ; Make the GUI window the last found window for use by the line below
  ; Also keep the window on top of other windows and don't show in the task bar
  Gui, Capture:+LastFound +AlwaysOnTop -Caption +ToolWindow

  ; Set the box opacity
  WinSet, Transparent, %captureBoxAlphaScaled%

  ; Update the active capture point
  updateActiveCoordWithMousePos()

  ; Bound the capture box coordinates
  if(topLeftCoordActive)
  {
    if(x1 + 1 > x2)
      x1 := x2 - 1

    if(y1 + 1 > y2)
      y1 := y2 - 1
  }
  else
  {
    if(x2 - 1 < x1)
      x2 := x1 + 1

    if(y2 - 1 < y1)
      y2 := y1 + 1
  }

  ; Display the capture box
  Gui, Capture:Show, % "X" x1 " Y" y1 " W" x2-x1 " H" y2-y1

  ; Handle the case where the capture ended but the timer still had one update left
  if(captureMode == 0)
  {
    Gui, Capture:Destroy
    return
  }

return ; UpdateCaptureBox:


; Start capture mode
startCaptureMode()
{
  global

  Gui, VoiceStatus:Destroy
  Gui, VoiceResults:Destroy
  Gui, Preview:Destroy
  Gui, Capture:Destroy

  ; End voice capture mode for the case were the user interrupted
  ; a voice capture with an OCR capture
  voiceCaptureMode = 0

  ; Activate hotkeys that are only needed in capture mode
  turnOnHotKeys()

  ; Set the capture mode flag
  captureMode = 1

  ; Set the active capture corner to the bottom-right corner
  topLeftCoordActive = 0

  ; Initialize the capture box coordinates
  MouseGetPos, x1, y1
  MouseGetPos, x2, y2

  ; Create a timer to update the capture box
  SetTimer, UpdateCaptureBox, %BOX_UPDATE_RATE%

  ; Make the first update immediate rather than waiting for the timer
  Gosub, UpdateCaptureBox

  if(previewBoxEnabled)
  {
    ; Create the preview window (with no text)
    Gui, Preview:Font, s%previewBoxFontSize%, %previewBoxFont%
    Gui, Preview:Margin, 3, 3
    Gui, Preview:Add, Text, vPreviewText X0 X0 c%previewBoxTextColor%
    Gui, Preview:Color, %previewBoxBackgroundColor%
    Gui, Preview:+LastFound +AlwaysOnTop -Caption +ToolWindow
    WinSet, Transparent, %previewBoxAlphaScaled%
    Gui, Preview:Show, X0 Y0

    ; Start a one-time timer to update the preview
    SetTimer, UpdatePreviewBox, -%PREVIEW_UPDATE_RATE%
  }

} ; startCaptureMode


; End COR capture mode
endCaptureMode()
{
  global

  ; Clear the capture mode flag
  captureMode = 0

  ; Stop the timer that updates the capture box
  SetTimer, UpdateCaptureBox, Off

  ; Stop the timer that updates the preview box
  SetTimer, UpdatePreviewBox, Off

  ; Remove the capture box
  Gui, Capture:Destroy

  ; Remove the preview box
  Gui, Preview:Destroy

  ; Deactivate hotkeys that are only needed in capture mode
  turnOffHotKeys()

} ; endCaptureMode


; Update the active capture coordinates with the current mouse position
updateActiveCoordWithMousePos()
{
  MouseGetPos, x, y

  setActiveCaptureCoords(x, y)

} ; updateActiveCoordWithMousePos


; Nudge the non-active capture coordinates
nudgeNonActiveCaptureCoords(x, y)
{
  global

  if(topLeftCoordActive)
  {
    x2 += x
    y2 += y

    if(x2 - 1 < x1)
      x2 := x1 + 1

    if(y2 - 1 < y1)
      y2 := y1 + 1
  }
  else
  {
    x1 += x
    y1 += y

    if(x1 + 1 > x2)
      x1 := x2 - 1

    if(y1 + 1 > y2)
      y1 := y2 - 1
  }
} ; nudgeNonActiveCaptureCoords


; Set the active capture coordinates
setActiveCaptureCoords(x, y)
{
  global

  if(topLeftCoordActive)
  {
    x1 := x
    y1 := y

    if(moveBothCorners)
    {
      x2 := x1 + bothCornersWidth
      y2 := y1 + bothCornersHeight
    }
  }
  else
  {
    x2 := x
    y2 := y

    if(moveBothCorners)
    {
      x1 := x2 - bothCornersWidth
      y1 := y2 - bothCornersHeight
    }
  }
} ; setActiveCaptureCoords


; Capture the screen and OCR it
; Parameters: integers; The screen coordinates that form the rectangle to capture
; Return Value: The OCR'd text
captureAndOCR(x1, y1, x2, y2)
{
  global

  captureFile = Output/screen_capture.bmp ; Name of the captured image
  dictionaryCode := ocrDicTable[dictionary]
  ocrUtil := dic2OcrUtil(dictionary)
  japConfigArg =

  ; Configure arguments based on OCR util
  if(ocrUtil == OCR_UTIL_NHOCR)
  {
    EnvSet, NHOCR_DICCODES, %dictionaryCode% ; Set the dictionary for the OCR tool

    ocrOutputFile = Output/ocr.txt

    if(ocrPreProcessing)
    {
      ocrInputFile = Output/ocr_in.pbm
    }
    else
    {
      ocrInputFile = Output/ocr_in.pgm
    }

    ocrUtilArgs = -block -o %ocrOutputFile% %ocrInputFile%
  }
  else ; Tesseract
  {
    ocrOutputFile = Output/ocr
    ocrInputFile = Output/ocr_in.tif

    ; If the vertical text direction and Chinese or Japanese dictionary
    if((dictionary == "Japanese (Tesseract)")
       || (dictionary == "Chinese - Simplified (Tesseract)")
       || (dictionary == "Chinese - Traditional (Tesseract)"))
    {
      ; Use a Japanese specific config
      japConfigArg = jap_config

      if(textDirection == "Vertical")
      {
        ; Assume a single uniform block of vertically aligned text.
        psmOption = -psm 5
      }
      else
      {
        ; Assume a single uniform block of text.
        psmOption = -psm 6
      }
    }
    else ; Horizontal text or not a Chinese or Japanese dictionary
    {
      ; Fully automatic page segmentation, but no OSD (Note: OSD is orientation detection). (Default)
      psmOption = -psm 3
    }

    whitelistArg =

    ; Determine if the whitelist should be used
    if (StrLen(whitelist) != 0)
    {
      whitelistArg = %WHITELIST_FILE%
    }

    ocrUtilArgs = %ocrInputFile% %ocrOutputFile% -l %dictionaryCode% %psmOption% %whitelistArg% %japConfigArg%
  }

  ; Form the screen capture rectangle
  captureRect = %x1%, %y1%, %x2%, %y2%

  ; Capture the selected portion of the screen
  CaptureScreen(captureRect, False, captureFile)
  
  ; Negate image? 0 = No, 1 = Yes, 2 = Auto  
  if(ocrPreProcessing)
  {
    negateArg = 2
  }
  else
  {
    negateArg = 0
  }
  
  ; Scale image?
  if(scaleFactor == 1.0)
  {
    performScaleArg = 0
  }
  else
  {
    performScaleArg = 1
  }

  ; Pre-process file (unsharp mask + ocrPreProcessing) to improve OCR
  RunWait, %LEPTONICA_UTIL% %captureFile%  %ocrInputFile%  %negateArg% 0.5  %performScaleArg% %scaleFactor%  %ocrPreProcessing% 5 2.5  %ocrPreProcessing% 2000 2000 0 0 0.0, , Hide

  ; Run OCR
  RunWait, %ocrUtil%  %ocrUtilArgs%, , Hide

  ; Read the OCR'd text from the OCR tools output
  ocrOutputFile = Output/ocr.txt
  FileRead, ocrText, %ocrOutputFile%

  return ocrText
} ; captureAndOCR


; Replace tokens with special characters
replaceSpecialChars(text)
{
  global

  StringReplace, text, text, ${cr}, `r, All
  StringReplace, text, text, ${lf}, `n, All
  StringReplace, text, text, ${tab}, %A_TAB%, All

  return text
} ; replaceSpecialChars


; Save the settings to settings.ini
saveSettings()
{
  global

  ; Output
  IniWrite, %saveToClipboard%, %SETTINGS_FILE%, Output, SaveToClipboard
  IniWrite, %sendToCursor%, %SETTINGS_FILE%, Output, SendToCursor
  IniWrite, %popupWindow%, %SETTINGS_FILE%, Output, PopupWindow
  IniWrite, %sendToControl%, %SETTINGS_FILE%, Output, SendToControl

  ; OCR Specific
  IniWrite, %ocrPreProcessing%, %SETTINGS_FILE%, OCRSpecific, OcrPreProcessing
  IniWrite, %dictionary%, %SETTINGS_FILE%, OCRSpecific, Dictionary
  IniWrite, %textDirection%, %SETTINGS_FILE%, OCRSpecific, TextDirection

  ; VoiceSpecific
  IniWrite, %voiceLanguage%, %SETTINGS_FILE%, VoiceSpecific, VoiceLanguage

} ; saveSettings


; Create the hotkeys based on user preference.
createHotKeys()
{
  global

  ; Hotkeys cannot be blank, so assign one if needed
  if(StrLen(startAndEndCaptureKey) == 0)
    startAndEndCaptureKey = #q
  if(StrLen(dictionary1Key) == 0)
    dictionary1Key = #1
  if(StrLen(dictionary2Key) == 0)
    dictionary2Key = #2
  if(StrLen(dictionary3Key) == 0)
    dictionary3Key = #3
  if(StrLen(ocrPreprocessingKey) == 0)
    ocrPreprocessingKey = #b
  if(StrLen(textDirectionToggleKey) == 0)
    textDirectionToggleKey = #w
  if(StrLen(endOnlyCaptureKey) == 0)
    endOnlyCaptureKey = LButton
  if(StrLen(ToggleActiveCaptureCornerKey) == 0)
    toggleActiveCaptureCornerKey = Space
  if(StrLen(MoveCaptureKey) == 0)
    moveCaptureKey = RButton
  if(StrLen(cancelCaptureKey) == 0)
    cancelCaptureKey = Esc
  if(StrLen(nudgeLeftKey) == 0)
    nudgeLeftKey = Left
  if(StrLen(nudgeRightKey) == 0)
    nudgeRightKey = Right
  if(StrLen(nudgeUpKey) == 0)
    nudgeUpKey = Up
  if(StrLen(nudgeDownKey) == 0)
    nudgeDownKey = Down
  if(StrLen(startVoiceCapture) == 0)
    startVoiceCapture = #a
  if(StrLen(voiceLanguageToggleKey) == 0)
    voiceLanguageToggleKey = #4

  HotKey, %startAndEndCaptureKey%, Capture
  HotKey, %dictionary1Key%, HandleKeyDictionary1
  HotKey, %dictionary2Key%, HandleKeyDictionary2
  HotKey, %dictionary3Key%, HandleKeyDictionary3
  HotKey, %ocrPreprocessingKey%, HandleOcrPreprocessingKey
  HotKey, %textDirectionToggleKey%, HandleKeyTextDirectionToggle
  HotKey, %startVoiceCapture%, CaptureVoice
  HotKey, %voiceLanguageToggleKey%, HandleKeyVoiceLanguage

  ; The following hotkeys are created but initialized to a disabled state
  HotKey, %endOnlyCaptureKey%, Capture, Off
  HotKey, %toggleActiveCaptureCornerKey%, ToggleActiveCaptureCorner, Off
  HotKey, %moveCaptureKey%, StartMoveCapture, Off
  HotKey, %moveCaptureKey% UP, EndMoveCapture, Off
  HotKey, %cancelCaptureKey%, CancelCapture, Off
  HotKey, %nudgeLeftKey%, NudgeLeftKey, Off
  HotKey, %nudgeRightKey%, NudgeRightKey, Off
  HotKey, %nudgeUpKey%, NudgeUpKey, Off
  HotKey, %nudgeDownKey%, NudgeDownKey, Off

} ; createHotKeys


; Active hotkeys that are only needed in capture mode
turnOnHotKeys()
{
  global

  HotKey, %endOnlyCaptureKey%, On, On
  HotKey, %toggleActiveCaptureCornerKey%, On, On
  HotKey, %moveCaptureKey%, On, On
  HotKey, %moveCaptureKey% UP, On, On
  HotKey, %cancelCaptureKey%, On, On
  HotKey, %nudgeLeftKey%, On, On
  HotKey, %nudgeRightKey%, On, On
  HotKey, %nudgeUpKey%, On, On
  HotKey, %nudgeDownKey%, On, On

} ; turnOnHotKeys


; Deactivate hotkeys that are only needed in capture mode
turnOffHotKeys()
{
  global

  HotKey, %endOnlyCaptureKey%, Off
  HotKey, %toggleActiveCaptureCornerKey%, Off
  HotKey, %moveCaptureKey%, Off
  HotKey, %moveCaptureKey% UP, Off
  HotKey, %cancelCaptureKey%, Off
  HotKey, %nudgeLeftKey%, Off
  HotKey, %nudgeRightKey%, Off
  HotKey, %nudgeUpKey%, Off
  HotKey, %nudgeDownKey%, Off

} ; turnOffHotKeys


; Process the text before it is output.
; Parameters:
;   text - string; The OCR'd/Voice Recognized text
processTextBeforeOutput(text)
{
  global

  ; Add special characters to Prepend Text
  StringLen, length, prependText
  if(length > 0)
  {
    prependText := replaceSpecialChars(prependText)
  }

  ; Add special characters to Append Text
  StringLen, length, appendText
  if(length > 0)
  {
    appendText := replaceSpecialChars(appendText)
  }

  ; Append/Prepend text to the OCR'd text
  text = %prependText%%text%%appendText%

  return text

} ; processTextBeforeOutput


; Output the OCR'd text based on user-defined settings (default is clipboard)
; Parameters:
;   text - string; The OCR'd/Voice Recognized text
doOutput(text)
{
  global

  ; Do final text processing (append text, prepend text, etc.)
  text := processTextBeforeOutput(text)

  ; Save the text to the clipboard
  if(saveToClipboard)
  {
    clipboard = %text%
  }

  ; Output the text to whichever control contains the blinking cursor.
  if(sendToCursor)
  {
    outputSendToCursor(text)
  }

  ; Show the text in a pop-up window
  if(popupWindow)
  {
    openOutputPopup(text, popupWindowWidth, popupWindowHeight)
  }

  ; Send the text to control
  if(sendToControl)
  {
    outputSendToControl(text)
  }

} ; doOutput


; Output the text to whichever control contains the blinking cursor.
; Parameters:
;   string - text; The OCR'd/Voice Recognized text
outputSendToCursor(text)
{
  global

  ; Send a command to the control before sending the text
  if(sendToCursorApplyBeforeAndAfterCommands && (StrLen(controlSendCommandBefore) > 0))
  {
    Send, %controlSendCommandBefore%
    Sleep, 25
  }

  ; Save the current contents of the clipboard so we can restore it later.
  ; This will also save things like pictures and formatting.
  clipSaved := ClipboardAll

  ; Put text in clipboard and paste the text to the control
  clipboard = %text%
  Sleep, 50
  Send, ^v
  Sleep, 50

  ; Restore the original clipboard
  clipboard := clipSaved

  ; Send a command to the control after sending the text
  if(sendToCursorApplyBeforeAndAfterCommands && (StrLen(controlSendCommandAfter) > 0))
  {
    Sleep, 25
    Send, %controlSendCommandAfter%
  }
} ; outputSendToCursor


; Output the text to control
; Parameters:
;   string - text; The OCR'd/Voice Recognized text
outputSendToControl(text)
{
  global

  ; Send a command to the control before sending the text
  if(StrLen(controlSendCommandBefore) > 0)
  {
    ControlSend, %controlClassNN%, %controlSendCommandBefore%, %controlWindowTitle%
    Sleep, 25
  }

  ; Send the OCR'd text to control
  if(replaceControlText)
  {
    ; Replace the text
    Sleep, 25
    ControlSetText, %controlClassNN%, %text%, %controlWindowTitle%
  }
  else
  {
    ; Send the text
    ControlSend, %controlClassNN%, %text%, %controlWindowTitle%
  }

  ; Send a command to the control after sending the text
  if(StrLen(controlSendCommandAfter) > 0)
  {
    Sleep, 25
    ControlSend, %controlClassNN%, %controlSendCommandAfter%, %controlWindowTitle%
  }

} ; outputSendToControl


; Configure the tray
configureTray()
{
  global

  ; Remove items placed automatically by AutoHotKey
  Menu, TRAY, NoStandard

  ; Set the tray icon
  Menu, TRAY, Icon, Capture2Text.ico

  ; Set the tray tooltip
  Menu, TRAY, Tip, %PROG_NAME%

  ; Add  Preferences...
  Menu, TRAY, Add, &Preferences..., MenuHandlerPreferences

  ; Add separator
  Menu, TRAY, Add

  ; Add  Save to Clipboard
  Menu, TRAY, Add, Save to &Clipboard, MenuHandlerSaveToClipboard

  ; Add Show Popup Window
  Menu, TRAY, Add, Show &Popup Window, MenuHandlerPopupWindow

  ; Add Send to Cursor
  Menu, TRAY, Add, Send to C&ursor, MenuHandlerSendToCursor

  ; Add separator
  Menu, TRAY, Add

  ; Add OCR Language items
  enum := installedDics._NewEnum()
  while enum[k, v]
  {
    ; Note: k = Language, v = Dic code
    menuItem := k
    Menu, MenuItemOCRLangSettings, Add, %menuItem%, MenuHandlerLang
  }

  ; Add OCR Language
  Menu, TRAY, Add, OCR &Language, :MenuItemOCRLangSettings

  ; Add voice language items
  enum := voiceTable._NewEnum()
  while enum[k, v]
  {
    ; Note: k = Language, v = voice code
    menuItem := k
    Menu, MenuItemVoiceLangSettings, Add, %menuItem%, MenuHandlerVoiceLang
  }

  ; Speech Recognition Language
  Menu, TRAY, Add, Speech &Recognition Language, :MenuItemVoiceLangSettings

  ; Add separator
  Menu, TRAY, Add

  ; Add Help...
  Menu, TRAY, Add, &Help..., MenuHandlerHelp

  ; Add About...
  Menu, TRAY, Add, &About..., MenuHandlerAbout

  ; Add Exit
  Menu, TRAY, Add, E&xit, HandleExit

  ; Check any menu item that needs to be checked at startup
  checkMenuItemsAtStartup()

} ; configureTray


; Check any menu item that needs to be checked at startup
checkMenuItemsAtStartup()
{
  global

  ; OCR Language menu
  uncheckLangMenuItems()
  checkLangMenuItem(dictionary)

  ; Voice Language menu
  uncheckVoiceLangMenuItems()
  checkVoiceLangMenuItem(voiceLanguage)

  ; Uncheck all output options
  Menu, TRAY, Uncheck, Save to &Clipboard
  Menu, TRAY, Uncheck, Send to C&ursor
  Menu, TRAY, Uncheck, Show &Popup Window

  if(saveToClipboard)
    Menu, TRAY, Check, Save to &Clipboard

  if(sendToCursor)
    Menu, TRAY, Check, Send to C&ursor

  if(popupWindow)
    Menu, TRAY, Check, Show &Popup Window

} ; checkMenuItemsAtStartup


; Check a language menu item
; langMenuItem is the language (not the dict code)
checkLangMenuItem(langMenuItem)
{
  global
  uncheckLangMenuItems()
  Menu, MenuItemOCRLangSettings, Check, %langMenuItem%
} ; checkLangMenuItem


; Check a voice language menu item
; langMenuItem is the language (not the voice code)
checkVoiceLangMenuItem(langMenuItem)
{
  global
  uncheckVoiceLangMenuItems()
  Menu, MenuItemVoiceLangSettings, Check, %langMenuItem%
} ; checkVoiceLangMenuItem


; Uncheck all language menu items
uncheckLangMenuItems()
{
  global

  enum := installedDics._NewEnum()
  while enum[k, v]
  {
    menuItem := k
    Menu, MenuItemOCRLangSettings, Uncheck, %menuItem%

    ; Sleep or else the compiled .exe version won't remove the checkbox
    ;Sleep, 10
  }

} ; uncheckLangMenuItems


; Uncheck all voice language menu items
uncheckVoiceLangMenuItems()
{
  global

  enum := voiceTable._NewEnum()
  while enum[k, v]
  {
    menuItem := k
    Menu, MenuItemVoiceLangSettings, Uncheck, %menuItem%

    ; Sleep or else the compiled .exe version won't remove the checkbox
    ;Sleep, 10
  }

} ; uncheckVoiceLangMenuItems


; Get the OCR tool that goes with a dictionary
; inDictionary - The language (not the dic code)
dic2OcrUtil(inDictionary)
{
  global

  if(inDictionary == "Japanese (NHocr)" or inDictionary == "Chinese (NHocr)")
  {
    theOcrUtil = %OCR_UTIL_NHOCR%
  }
  else
  {
    theOcrUtil = %OCR_UTIL_TESSERACT%
  }

  return theOcrUtil

} ; dic2OcrUtil


; Replace certain characters in the OCR'd text
replaceOCRText(ocrText, force=false)
{
  global

  if(!preserveNewlines || force)
  {
    ; If Japanese or Chinese remove newlines, otherwise, replace them with spaces
    if(  (dictionary == "Japanese (Tesseract)")
      || (dictionary == "Japanese (NHocr)")
      || (dictionary == "Chinese (NHocr)")
      || (dictionary == "Chinese - Simplified (Tesseract)")
      || (dictionary == "Chinese - Traditional (Tesseract)"))
    {
      StringReplace, ocrText, ocrText, `r`n, , All
      StringReplace, ocrText, ocrText, `r, , All
      StringReplace, ocrText, ocrText, `n, , All
    }
    else ; Non-Japanese or Chinese language
    {
      StringReplace, ocrText, ocrText, `r`n, %A_Space%, All
      StringReplace, ocrText, ocrText, `r, %A_Space%, All
      StringReplace, ocrText, ocrText, `n, %A_Space%, All
    }
  }

  ; Trim whitesapce from start and end
  ocrText := Trim(ocrText, "`n")
  ocrText := Trim(ocrText, "`r")
  ocrText := Trim(ocrText, " ")

  ocrText := performsubstitutions(ocrText)

  return ocrText

} ; replaceOCRText()


; Read in the substitutions file. Parses the "All:" and current OCR language sections
readsubstitutions()
{
 global

  ; Clear substitutions list
  subList := []

  ; File parse state
  ; 0 = Searching for All: section
  ; 1 = Looping through All: section
  ; 2 = Searching for current OCR language section
  ; 3 = Looping through current OCR language section
  ; 4 = Done
  state = 0

  ; Read the substitutions file
  Loop, read, %SUBSTITUTIONS_FILE%
  {
    if(state == 0)
    {
      if(A_LoopReadLine == "All:")
      {
        state = 1
      }
    }
    else if((state == 1) || (state == 3))
    {
      StringLeft, firstChar, A_LoopReadLine, 1

      ; If the line is blank or is a comment
      if((StrLen(A_LoopReadLine) == 0) || (firstChar == "#"))
      {
        continue
      }

      ; If the line contains "="
      if(InStr(A_LoopReadLine, "="))
      {
        subList.Insert(A_LoopReadLine)
      }
      else
      {
        if(state == 1)
        {
          state = 2
        }
        else
        {
          state = 4
        }
      }
    }
    else if(state == 2)
    {
      dictLine = %dictionary%:

      if(A_LoopReadLine == dictLine)
      {
        state = 3
      }
    }
    else if(state == 4)
    {
      break
    }
  }

} ; readsubstitutions


; Perform substitutions on the provided text.
; Global inputs:
;   subList
performsubstitutions(text)
{
  global

  ; Loop through substitutions list
  Loop % subList.MaxIndex()
  {
    ; Get current substitution
    curSub := % subList[A_Index]

    ; Extract the from and to portions of the substitution
    StringSplit, fields, curSub, =
    replaceFrom = %fields1%
    replaceTo = %fields2%

    ; Replace special tokens
    StringCaseSense, On
    StringReplace, replaceFrom, replaceFrom, `%cr`%, `r, All
    StringReplace, replaceTo, replaceTo, `%cr`%, `r, All

    StringReplace, replaceFrom, replaceFrom, `%eq`%, =, All
    StringReplace, replaceTo, replaceTo, `%eq`%, =, All

    StringReplace, replaceFrom, replaceFrom, `%lf`%, `n, All
    StringReplace, replaceTo, replaceTo, `%lf`%, `n, All

    StringReplace, replaceFrom, replaceFrom, `%perc`%, `%, All
    StringReplace, replaceTo, replaceTo, `%perc`%, `%, All

    StringReplace, replaceFrom, replaceFrom, `%space`%, %A_Space%, All
    StringReplace, replaceTo, replaceTo, `%space`%, %A_Space%, All

    StringReplace, replaceFrom, replaceFrom, `%tab`%, %A_Tab%, All
    StringReplace, replaceTo, replaceTo, `%tab`%, %A_Tab%, All

    ; Perform substitution
    StringReplace, text, text, %replaceFrom%, %replaceTo%, All
    StringCaseSense, Off
  }

  return text

} ; performsubstitutions


; Handle a language menu item from Settings | OCR Language
MenuHandlerLang:
  switchToDic(A_ThisMenuItem)
return ; MenuHandlerLang:

MenuHandlerVoiceLang:
  voiceLanguage = %A_ThisMenuItem%
  checkVoiceLangMenuItem(A_ThisMenuItem)
return ; MenuHandlerVoiceLang


; Switch to the given dictionary. This sets the dictionary, menu check mark, and OCR util
switchToDic(dic)
{
  global
  checkLangMenuItem(dic)
  dictionary := dic

  ; Read the substitutions file for the new language
  readsubstitutions()

} ; switchToDic


; Toggle text direction
toggleTextDirection()
{
  global

  if(textDirection == "Vertical")
  {
    textDirection = Horizontal
  }
  else
  {
    textDirection = Vertical
  }
} ; toggleTextDirection


; Toggle voice capture language
toggleVoiceLanguage()
{
  global

  if(voiceLanguage == voiceLanguage1)
  {
    voiceLanguage = %voiceLanguage2%
  }
  else
  {
    voiceLanguage = %voiceLanguage1%
  }
} ; toggleVoiceLanguage


; Toggle OCR pre-processing
toggleOcrPreprocessing()
{
  global

  if(ocrPreProcessing)
  {
    ocrPreProcessing = 0
    SetTrayTip("OCR Pre-Processing OFF", 1000)
  }
  else
  {
    ocrPreProcessing = 1
    SetTrayTip("OCR Pre-Processing ON", 1000)
  }
} ; toggleOcrPreprocessing


; Set the tray tip
SetTrayTip(txt, timeout)
{
  global

  TrayTip, , %txt%
  SetTimer, RemoveTrayTip, %timeout%

} ; SetTrayTip


; Used with SetTrayTip to remove the tray tip
RemoveTrayTip:
  SetTimer, RemoveTrayTip, Off
  TrayTip
return ; RemoveTrayTip:


; Handle Save to Clipboard
MenuHandlerSaveToClipboard:
  if(saveToClipboard)
  {
    Menu, TRAY, Uncheck, %A_ThisMenuItem%
    saveToClipboard = 0
  }
  else
  {
    Menu, TRAY, Check, %A_ThisMenuItem%
    saveToClipboard = 1
  }
return ; MenuHandlerSaveToClipboard:


; Handle Send to Cursor
MenuHandlerSendToCursor:
  if(sendToCursor)
  {
    Menu, TRAY, Uncheck, %A_ThisMenuItem%
    sendToCursor = 0
  }
  else
  {
    Menu, TRAY, Check, %A_ThisMenuItem%
    sendToCursor = 1
  }
return ; MenuHandlerSendToCursor:


; Handle Show Popup Window
MenuHandlerPopupWindow:
  if(popupWindow)
  {
    Menu, TRAY, Uncheck, %A_ThisMenuItem%
    popupWindow = 0
  }
  else
  {
    Menu, TRAY, Check, %A_ThisMenuItem%
    popupWindow = 1
  }
return ; MenuHandlerPopupWindow:


; Handle Preferences...
MenuHandlerPreferences:
  saveSettings()
  openSettingsDialog()

  ; Set timer to check result of the settings dialog
  SetTimer, CheckSettingsDlgClosed, 50
return ; MenuHandlerPreferences


; Used to check the result of the settings dialog and take the appropriate action
CheckSettingsDlgClosed:
  if((settingsDlgResult == SETTING_DLG_INACTIVE)
    || (settingsDlgResult == SETTING_DLG_CANCEL))
  {
    settingsDlgResult := SETTING_DLG_INACTIVE
    SetTimer, CheckSettingsDlgClosed, Off
  }
  else if(settingsDlgResult == SETTING_DLG_OK)
  {
    ; Disable all current hotkeys. If we don't do this, the old hotkeys
    ; will remain active alongside the new hotkeys.
    HotKey, %startAndEndCaptureKey%, Off
    HotKey, %dictionary1Key%, Off
    HotKey, %dictionary2Key%, Off
    HotKey, %dictionary3Key%, Off
    HotKey, %ocrPreprocessingKey%, Off
    HotKey, %textDirectionToggleKey%, Off
    HotKey, %startVoiceCapture%, Off
    HotKey, %voiceLanguageToggleKey%, Off
    HotKey, %endOnlyCaptureKey%, Capture, Off
    HotKey, %toggleActiveCaptureCornerKey%, ToggleActiveCaptureCorner, Off
    HotKey, %moveCaptureKey%, StartMoveCapture, Off
    HotKey, %moveCaptureKey% UP, EndMoveCapture, Off
    HotKey, %cancelCaptureKey%, CancelCapture, Off
    HotKey, %nudgeLeftKey%, NudgeLeftKey, Off
    HotKey, %nudgeRightKey%, NudgeRightKey, Off
    HotKey, %nudgeUpKey%, NudgeUpKey, Off
    HotKey, %nudgeDownKey%, NudgeDownKey, Off

    ; Read the settings that where set by the Settings dialog
    readSettings()

    ; Determine checkmark in the tray context menu
    checkMenuItemsAtStartup()

    ; Re-create the hotkeys with the new settings
    createHotKeys()

    ; Enable the always on hotkeys. If we don't do this, the hotkeys that
    ; didn't change would be disabled due to the above code.
    HotKey, %startAndEndCaptureKey%, On
    HotKey, %dictionary1Key%, On
    HotKey, %dictionary2Key%, On
    HotKey, %dictionary3Key%, On
    HotKey, %ocrPreprocessingKey%, On
    HotKey, %textDirectionToggleKey%, On
    HotKey, %startVoiceCapture%, On
    HotKey, %voiceLanguageToggleKey%, On

    settingsDlgResult := SETTING_DLG_INACTIVE
    SetTimer, CheckSettingsDlgClosed, Off

    ; Read the substitutions file in case language changed
    readsubstitutions()
  }
return ; CheckSettingsDlgClosed:


; Handle Help...
MenuHandlerHelp:
  Run, http://capture2text.sourceforge.net/
return ; MenuHandlerHelp


; Handle About...
MenuHandlerAbout:
  MsgBox, 64, %PROG_NAME%, %PROG_NAME%`r`nVersion: %VERSION%`r`nAuthor: %AUTHOR%`r`nContact: %CONTACT%%A_TAB%
return ; MenuHandlerAbout


; Handle program closing
HandleExit:
  saveSettings()
  ExitApp ; Exit this program
return ; HandleExit

