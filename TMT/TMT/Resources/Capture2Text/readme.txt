Capture2Text Readme
--------------------------------------------------------------------------------

--------------------------------------------------------------------------------
What is Capture2Text?
--------------------------------------------------------------------------------

Capture2Text enables users to do the following:

  * Optical Character Recognition (OCR)

    Allows the user to quickly snapshot a small portion of the screen, OCR it
    and (by default) save the result to the clipboard.

  * Speech Recognition (experimental)

    Using speech recognition the user can speak into their microphone and
    Capture2Text will convert the speech to text. If the speech recognition
    technology is not 100% sure, Capture2Text will present the user with a
    list of the most likely transcriptions. The selected result will (by default)
    be copied to the clipboard.

--------------------------------------------------------------------------------
How to Launch Capture2Text (no installation required):
--------------------------------------------------------------------------------

1) Unzip the contents of the zip file. Make sure that there are no Asian or
   other non-ASCII characters in the path where you unzipped it. Also, if you
   are on Windows 7, don't unzip it to the Program Files directory (this will
   avoid issues related to write privileges).

2) Double-click on Capture2Text.exe. You should see the Capture2Text icon on the
   bottom-right of your screen (though it might be hidden in which case you
   will have to click on the "Show hidden icons" arrow).

--------------------------------------------------------------------------------
OCR Language Support:
--------------------------------------------------------------------------------

Capture2Text can OCR the following languages:

  Afrikaans               Finnish             Maltese
  Albanian                Frankish            Norwegian
  Ancient Greek           French              Polish
  Arabic                  Galician            Portuguese
  Azerbaijani             German              Romanian
  Basque                  Greek               Russian
  Belarusian              Hebrew              Serbian
  Bengali                 Hindi               Slovakian
  Bulgarian               Hungarian           Slovenian
  Catalan                 Icelandic           Spanish
  Cherokee                Indonesian          Swahili
  Chinese (Simplified)    Italian             Swedish
  Chinese (Traditional)   Japanese            Tagalog
  Croatian                Kannada             Tamil
  Czech                   Korean              Telugu
  Danish                  Latvian             Thai
  Dutch                   Lithuanian          Turkish
  English                 Macedonian          Ukrainian
  Esperanto               Malay               Vietnamese
  Estonian                Malayalam

By default only English, French, German, Italian, Japanese and Spanish are
installed.

To acquire other languages:

  1) Download the appropriate OCR language dictionaries from:
     http://code.google.com/p/tesseract-ocr/downloads/list
     These files end in ".tar.gz" (ex. tesseract-ocr-3.02.rus.tar.gz).

  2) Open the ".tar.gz" file you just downloaded with 7-Zip or similar
     decompression software and navigate to the directory that has the file
     that ends in ".traineddata".

  3) Drag the ".traineddata" file (and any other file in this directory) to this
     path in the Capture2Text directory: Capture2Text\Utils\tesseract\tessdata

  4) Restart Capture2Text

  Note: Arabic and Hindi are more CPU intensive and will thus be slower to OCR.

To install the Chinese (NHocr) language pack:

  1) Download nhocr-0.18-dic-zh_CN-091226.tar.gz from
     https://code.google.com/p/nhocr/downloads/list

  2) Open it with 7-Zip and copy PLM-zh_CN.dic and cctable-zh_CN to
     Capture2Text\Utils\nhocr\Dic

  3) Restart Capture2Text

--------------------------------------------------------------------------------
OCR Usage:
--------------------------------------------------------------------------------

Press the OCR capture key (default: Windows Key + Q) to start the capture. Now,
using your mouse, resize the capture box over the area of the screen that you want
to OCR. A preview of the captured OCR'd text will appear in the top-left corner of
the screen. Press the capture key again or the left mouse button to complete the
capture. The captured screen area will be OCR'd and the textual result will be
stored in the clipboard by default.

To cancel an OCR capture, press ESC.

To move the capture box, hold down the right mouse button and drag the mouse.

To nudge the capture box, use the arrow keys.

To toggle the active capture box corner, press the space bar.

To change the OCR language, right-click the Capture2Text tray icon, select the
OCR Language option and then select the desired language.

To quickly switch between 3 languages, use the OCR language quick access keys:
Windows Key + 1, Windows Key + 2 and Windows Key + 3.

When the Tesseract versions of Chinese or Japanese is selected, you should
specify the text direction (vertical or horizontal) using the text
direction key: Windows Key + W. The text direction will not have any effect
on the NHocr Chinese and Japanese dictionaries.

(Advanced) To toggle OCR pre-processing, press Windows Key + B.

Using the Preferences dialog, you can change the following OCR settings:
  - OCR Hotkeys.
  - Current OCR Language.
  - The 3 Quick-Access OCR Languages.
  - Capture Box color and opacity.
  - Enable/Disable the preview box and change its colors, font and opacity.
  - Change the text direction (used for Chinese and Japanese).

--------------------------------------------------------------------------------
Speech Recognition Language Support:
--------------------------------------------------------------------------------

Capture2Text can perform speech recognition for the following languages:

  Afrikaans    French      Polish
  Chinese      German      Portuguese
  Czech        Italian     Russian
  Dutch        Japanese    Spanish
  English      Korean      Turkish

Note: The speech recognition feature is still experimental and may not
      work properly.

--------------------------------------------------------------------------------
Speech Recognition Usage:
--------------------------------------------------------------------------------

Press the speech recognition capture key (default: Windows Key + A) to start the
capture. You will see a box that says "Recording..." in the top-left corner of your
screen. Speak a word or phrase or sentence into your microphone. Capture2Text will
automatically recognize when you are done speaking and will display a box that
says "Analyzing...". The speech recognition will take a couple of seconds. When
the speech recognition is complete you will see a list of possible transcriptions
to choose from. When you choose a transcription, it will be stored in the clipboard
by default.

When the results windows is displayed, you can press Enter to select the first
transcription or use the number keys (1-9) to select the corresponding transcription.

To cancel a speech recognition capture, press ESC.

To change the speech recognition language, right-click the Capture2Text tray icon,
select the Speech Recognition Language option and then select the desired language.

To quickly toggle between 2 languages, use the speech recognition language hotkey:
Windows Key + 4

Using the Preferences dialog, you can change the following speech recognition settings:
  - Speech recognition Hotkeys.
  - Current speech recognition Language.
  - The 2 speech recognition languages to toggle between.
  - The properties of the Results window (font, color, number of results).
  - How much silence to wait for before recording stops.

--------------------------------------------------------------------------------
Output Options:
--------------------------------------------------------------------------------

By default, the OCR'd or speech recognized text will be placed in the clipboard.

You also have 3 more ways to output the text.

To send the text to a pop-up window you can right-click the Capture2Text tray
icon and select Show Popup Window.

To send the text to whichever textbox currently contains the blinking
cursor/I-beam, right-click the Capture2Text tray icon and select Send to Cursor.

(Advanced) To send the text directly to a window/control (for example, Notepad++),
first fill in the Send to Control settings in the Preferences dialog.

Using the Preferences dialog, you can change the following output settings:
  - Text to prepend/append to the captured text.
  - Enable/Disable outputting to the clipboard.
  - Enable/Disable outputting to a popup window.
  - Popup window properties (default width and height).
  - Enable/Disable sending the output text to the cursor.
  - Enable/Disable outputting to a control.
  - Additional command to send to the output control.

--------------------------------------------------------------------------------
Configuration:
--------------------------------------------------------------------------------

Right-click the Capture2Text tray icon in the bottom-right of your screen and
then select the "Preferences..." option to bring up the Preferences dialog.

--------------------------------------------------------------------------------
Substitutions:
--------------------------------------------------------------------------------

Sometimes Capture2Text consistently makes the same OCR mistakes such as
recognizing an "M" as "I\/|".

By editing the substitutions.txt file in the Capture2Text directory, you may
tell Capture2Text to substitute one text string for another text string.

Just find the appropriate language section and add one substitution
per line in this format:
  from_text = to_text

Example (adding 3 substitutions to the English section):
  English:
    I\/| = M
    >< = X
    some%space%text = some_text

To create a substitution regardless of language, add the substitution to
the "All:" section.

Special tokens and escape characters:
  %space% = Space character ( )
  %tab%   = Tab character (	)
  %eq%    = Equals (=)
  %perc%  = Percent sign (%)
  %lf%    = Linefeed character (\n)
  %cr%    = Carriage return character (\r)

You may disable a substitution by adding a "#" in front.

When done editing substitutions.txt, either restart Capture2Text or switch
language for the substitutions to take effect.

--------------------------------------------------------------------------------
Command Line Options:
--------------------------------------------------------------------------------

You may OCR the screen via command line by calling Capture2Text in this format:

Capture2Text.exe x1 y1 x2 y2 [output_file]

  Required arguments:
    x1 - X1-Coordinate of the screen
    y1 - Y1-Coordinate of the screen
    x2 - X2-Coordinate of the screen
    y2 - Y2-Coordinate of the screen

  Optional arguments:
    output_file - The OCR'd text will be written to this file if specified.

Capture2Text will read settings.ini to determine settings such as OCR language
and output options (clipboard, popup, etc.).

Examples:
  Capture2Text.exe 10 152 47 321 output.txt
  Capture2Text.exe 10 152 47 321

Note:
  Make sure that you wait for Capture2Text to finish processing before
  attempting to start a new instance of Capture2Text.

--------------------------------------------------------------------------------
Troubleshooting & FAQ:
--------------------------------------------------------------------------------

Issue #1: Capture2Text doesn't work at all. What can I do?

  Possible solutions:

  - Make sure that you have unzipped Capture2Text to a directory with no
    Asian or other non-ASCII characters. Search Google if you do not know how
    to unzip a file.

  - Make sure that you did not unzip Capture2Text to the Program Files directory.
    This will avoid possible issues related to write privileges.

  - Try unzipping Capture2Text to a very short path such as C:\Capture2Text.
    Some computers supposedly have issues with longer paths. I have never
    actually seen this happen though.

  - Make sure that your Anti-virus software is not blocking Capture2Text.
    Refer to the documentation that was bundled with your Anti-virus software.

  - Make sure that you have downloaded the latest version from SourceForge:
    http://sourceforge.net/projects/capture2text/files/Capture2Text/

  - Restart your computer.

  - Right-click Capture2Text.exe and select "Run as administrator". Capture2Text
    shouldn't need administrator privileges, but if all else fails it won't
    hurt to try.

  - Ask one of your grandchildren to help you.

  If none of these things work for you, I suggest deleting Capture2Text and
  looking for some other OCR software (do not ask me for recommendations).

Issue #2: Capture2Text stopped working.

  Just restart it. I have never actually seen this happen though.

Issue #3: The language that I'm interested in doesn't appear in the OCR language menu.

  Read the OCR Language Support section of this document to learn how to add new
  languages.

Issue #4: I don't see the Capture2Text tray icon.

  Perhaps Windows did you the disservice of hiding it for you. Click the "Show hidden icons"
  button (it looks like an triangle) to see the Capture2Text tray icon.

Issue #5: I've clicked on the Capture2Text tray icon but it doesn't do anything.

  Right-click it instead.

Issue #6: Capture2Text isn't properly OCR'ing some word or character.

  Since I don't maintain the OCR training files, there is nothing that I can
  do about it. If you have a technical background and a lot of free time, feel free
  to create your own training files:
  https://code.google.com/p/tesseract-ocr/wiki/TrainingTesseract3

Issue #7: Capture2Text isn't working on my Mac.

  Capture2Text is a Windows-only software. If you have a technical background,
  feel free to port it (but don't ask me to help).

Issue #8: Speech recognition isn't working very well.

  Speech recognition is an experimental feature and I might remove it
  in a future release of Capture2Text.

Issue #9: Where is the uninstaller?

  There isn't one. Capture2Text doesn't have an installer either. To remove
  Capture2Text from your computer, simply delete the Capture2Text directory.

--------------------------------------------------------------------------------
Notes About the OCR Languages:
--------------------------------------------------------------------------------

- "Japanese (NHocr)"
  - Uses the NHocr OCR tool.
  - ASCII characters are supported, but not reliable.
  - Kana has a higher than expected miss-rate.
  - Vertical text has a higher miss-rate than horizontal text.
  - Miss-rate increases with smaller characters.
  - Some kanji are harder to OCR than other kanji. Especially if the
    kanji has whitespace in between it's primitive elements.
  - Handwritten text is not supported.

- "Japanese (Tesseract)"
  - Uses the Tesseract OCR tool.
  - The notes from the "Japanese" section apply here except that ASCII
    characters and kana have a lower miss-rate.
  - You must select the text direction: vertical or horizontal. The default
    key for this is Windows Key + W.

- "Chinese (NHocr)"
  - This uses the NHocr OCR tool.
  - Most of the notes from "Japanese" section also apply here.

- "Chinese - Simplified (Tesseract)" and "Chinese - Traditional (Tesseract)"
  - These use the Tesseract OCR tool.
  - Most of the notes from "Japanese (Tesseract)" section also apply here.

- All other languages
  - These use the Tesseract OCR tool.
  - OCR is very good as long as the characters aren't too small (8-pt and below).

--------------------------------------------------------------------------------
Tips to Increase OCR Accuracy:
--------------------------------------------------------------------------------

- Make sure that what you are viewing is >= its original size. That is,
  make sure that your image viewer isn't zoomed out too much.

- Sometimes it helps shape the capture box very tightly around the text.

- Enable the OCR pre-preprocessing option in Preferences -> Output.

--------------------------------------------------------------------------------
How to Run Capture2Text from Source Code:
--------------------------------------------------------------------------------

1) Download and install AutoHotkey 32-bit Unicode from:
   http://ahkscript.org/download/

2) Copy the .ahk files from SourceCode/Capture2Text_AHK_Script to the root
   Capture2Text folder (the same folder that contains Capture2Text.exe)

3) Double-click Capture2Text.ahk.

--------------------------------------------------------------------------------
Contact:
--------------------------------------------------------------------------------

Christopher Brochtrup
cb4960@gmail.com

I do not provide technical support. Instead, see the Troubleshooting & FAQ section.

I will accept feature suggestions, but no promises that I will actually
implement your suggested features.

--------------------------------------------------------------------------------
Acknowledgments/Tools Used:
--------------------------------------------------------------------------------

Tesseract     - OCR engine.
NHocr         - Alternate OCR engine for Japanese and Chinese.
Leptonica     - Image processing and analysis library.
SoX           - Audio utility used here to capturing voice to FLAC file.
Google        - Voice recognition service.
wget          - Utility used here to send voice recognition request to Google.
ScreenCapture - AutoHotKey screen capture script.

--------------------------------------------------------------------------------
Related Tools for Japanese Language Learners:
--------------------------------------------------------------------------------

- JGlossator - http://jglossator.sourceforge.net/
  Automatically lookup Japanese words that you have OCR'd with Capture2Text.
  Supports de-inflected expressions, readings, audio pronunciation, example sentences,
  pitch accent, word frequency, kanji information, and grammar analysis.

- OCR Manga Reader - https://play.google.com/store/apps/details?id=com.cb4960.ocrmr&hl=en
  Free and open source Manga reader android app that allows you to quickly OCR and lookup
  Japanese words in real-time. There are no ads and no mysterious network permissions.
  Supports both EDICT and EPWING dictionaries.

--------------------------------------------------------------------------------
Version History:
--------------------------------------------------------------------------------

[Version 3.3 (3-02-2014)]
- More minor tweaks to the Preferences dialog.

[Version 3.2 (3-01-2014)]
- Minor tweaks to the Preferences dialog.

[Version 3.1 (2-28-2014)]
- Improved OCR accuracy through use of better image pre-preprocessing (leptonica_util).
- Now supports text and backgrounds of any color when OCR pre-processing is enabled.
  (In the previous version, only dark text on a light background was supported).
- Added option to place the preview text beside the capture box.
- Japanese (Tesseract) accuracy is now vastly improved through use of a Japanese-specific
  Tesseract config file. Also using this config file with Chinese (Tesseract).
- Using Tesseract v3.02.02 for Japanese (was v3.01).
- Replaced the binarize option with the OCR pre-processing option.
- Removed "Send to Control" from the right-click menu.
- Removed the Chinese (NHocr) language pack from default distribution. (You can
  still download it from https://code.google.com/p/nhocr/downloads/list).
- Added the Italian language pack to the default distribution.
- Removed setting of PreviewRemoveCaptureBox from the GUI.
- Removed ConvertImageFormat (replaced with leptonica_util).
- Now compiled with AutoHotkey 32-bit Unicode v1.1.14.03 (was v1.1.11.01).

[Version 3.0 (8-27-2013)]
- Added option to binarize captured image before sending it to the OCR engine.

[Version 2.5 (7-5-2013)]
- Updated NHocr from v0.20 to v0.21.
- Now compiled with Ahk2Exe v1.1.11.01 instead of v1.1.05.06.

[Version 2.4 (12-29-2012)]
- Added support for Arabic, Danish (Alternate), Esperanto (Alternate),
  German (Alternate) and Slovakian (Alternate).

[Version 2.3 (11-9-2012)]
- Added option to remove the capture box before a preview OCR. This is more
  accurate, particularly with NHocr, but causes the capture box to flicker.
- Changed the default image scale factor from 300% to 320% to meet Tesseract's
  minimum recommended DPI.
- When using Japanese, revert to Tesseract v3.01. It is MUCH more accurate than v3.02.02.
- Now passing a .ppm image to NHocr instead of a .pgm image to better handle
  non-grayscale captures.
- Increased update rate of the capture box to make it appear more fluid.
- Fixed text direction being ignored bug for Chinese/Japanese that was introduced in v2.2.
- Fixed bug that caused the capture box to stick around after it was supposed to
  be removed.

[Version 2.2 (11-4-2012)]
- Upgraded to Tesseract v3.02.02. For details, see:
  http://code.google.com/p/tesseract-ocr/wiki/ReleaseNotes
- Added whitelist option to the OCR tab.
- Simplified substitution tokens and fixed whitespace bug.

[Version 2.1 (10-7-2012)]
- Added the substitutions feature.
- Added command line options.

[Version 2.0 (3-10-2012)]
- Added the Preferences dialog. No more editing settings.ini by hand.
- The popup window is now multi-lined.
- Added option to preserve newline characters.
- Limited preview to 150 characters. A trailing "..." will appear if necessary.
- Added Speech Recognition Language option to right-click menu.
- Cleaned up the right-click menu.
- On the first run, inform user how to access the Preferences dialog.

[Version 1.10a (2-18-2012)]
- Removed GdiPlus.dll from distribution.

[Version 1.10 (12-31-2011)]
- Added preview box (and corresponding settings)

[Version 1.09 (11-10-2011)]
- Fixed speech recording stopping in the middle of a sentence.
- Fixed VoiceMaxResults not working correctly. Also increased to 9 as default.

[Version 1.08 (11-06-2011)]
- Upgraded Tesseract to version 3.01 (it has better vertical text support and
  doesn't ignore small captures as much)
- When using Tesseract Chinese or Japanese, you can now select the text
  direction (vertical or horizontal). To support this, added
  TextDirectionToggleKey and textDirection to settings.ini.
- Changed default for ScaleFactor from 4.0 to 3.0 in settings.ini.
- Changed menu text for Chinese and Japanese to reflect the OCR engine being used.

[Version 1.07 (11-05-2011)]
- Added voice recognition support via unofficial Google voice recognition service
- Added the "Send To Cursor" option to menu. The setting.ini file includes:
    SendToCursor
    SendToCursorApplyBeforeAndAfterCommands
- Renamed OCRAdjustment to OCRSpecific in settings.ini
- Moved the CaptureBox section in settings.ini to the OCRSpecific section
- Added VoiceSpecific to settings.ini. Section includes:
    VoiceMaxResults
    VoiceResultsWindowWidth
    VoiceResultsWindowFont
    VoiceResultsWindowFontSize
    VoiceSilenceBeforeStop
    VoiceLanguage
- Added StartVoiceCapture to Hotkey section in settings.ini
- Added VoiceLanguageToggleKey to Hotkey section in settings.ini
- Removed scaleFilter from settings.ini
- Removed the scaleFactor option from the menu (it's still in settings.ini)

[Version 1.06 (12-12-2010)]
- Added language quick access keys.
- For Chinese and Japanese delete newlines. For other languages replace
  newlines with spaces.

[Version 1.05 (12-04-2010)]
- Fixed issue where the checkboxes in the language menu wouldn't disappear.

[Version 1.04 (12-04-2010)]
- Added ability to move the capture box by right-clicking
- Added languages supported by the Tesseract OCR tool
- Created a right-click menu that allow the user to select language, output type,
  capture box settings and scale factor
- Removed unnecessary items from settings.ini

[Version 1.03 (11-27-2010)]
- Added ability to change dictionary when the Dictionary setting in settings.ini
- Added Chinese dictionary

[Version 1.02 (11-27-2010)]
- Changed CaptureKey to StartAndEndCaptureKey in settings.ini
- Added EndOnlyCaptureKey to settings.ini
- Added ToggleActiveCaptureCornerKey to setting.ini

[Version 1.01 (11-27-2010)]
- Added ReplaceControlText to settings.ini
- Added ability to use linefeeds, carriage returns and tabs in PrependText and AppendText
- Added an "About" item to the tray menu.
- Removed the capture box showing up in the taskbar
- Removed the PassThruKey settings in settings.ini. They are no longer needed.
- Changed the tray tooltip text
- Cleaned up code and put the ScreenCapture routines in a separate file

[Version 1.00 (11-26-2010)]
- Initial version

--------------------------------------------------------------------------------

