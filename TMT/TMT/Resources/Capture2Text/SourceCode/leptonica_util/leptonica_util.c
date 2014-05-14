/* 
-------------------------------------------------------------------------
Copyright (C) 2014 Christopher Brochtrup

This file is part of Leptonica Util.

Leptonica Util is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Leptonica Util is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Leptonica Util.  If not, see <http://www.gnu.org/licenses/>.
-------------------------------------------------------------------------
Name:        leptonica_util.c
Description: Perform image pre-processing before OCR.
Version:     1.1
Contact:     cb4960@gmail.com
-------------------------------------------------------------------------
To build, link with liblept168.lib.
-------------------------------------------------------------------------
*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "leptonica/allheaders.h"


#define MAX_FILE_LEN   512


/* Used to scale and binarize a bitmap. */
int main(int argc, char *argv[])
{
  char source_file[MAX_FILE_LEN] = "in.png";
  char dest_file[MAX_FILE_LEN]   = "out.png";

  int perform_negate          = 2;    /* 0 = no, 1 = yes, 2 = auto */
  l_float32 dark_bg_threshold = 0.5f; /* From 0.0 to 1.0, with 0 being all white and 1 being all black */

  int perform_scale           = 1;
  l_float32 scale_factor      = 3.5f;

  int perform_unsharp_mask    = 1;
  l_int32 usm_halfwidth       = 5;
  l_float32 usm_fract         = 2.5f;

  int perform_otsu_binarize   = 1;
  l_int32 otsu_sx             = 2000;
  l_int32 otsu_sy             = 2000;
  l_int32 otsu_smoothx        = 0;
  l_int32 otsu_smoothy        = 0;
  l_float32 otsu_scorefract   = 0.0f;

  l_int32 status              = -1;
  l_float32 border_avg        = -1;
  PIX *pixs                   = NULL;
  char *ext                   = NULL;

  /* Get args.
     leptonica_util.exe in.png out.png  2 0.5  1 3.5  1 5 2.5  1 2000 2000 0 0 0.0 */
  if(argc >= 16)
  {
    strcpy_s(source_file, MAX_FILE_LEN, argv[1]);
    strcpy_s(dest_file,   MAX_FILE_LEN, argv[2]);

    perform_negate        = atoi(argv[3]);
    dark_bg_threshold     = (float)atof(argv[4]);

    perform_scale         = atoi(argv[5]);
    scale_factor          = (float)atof(argv[6]);

    perform_unsharp_mask  = atoi(argv[7]);
    usm_halfwidth         = atoi(argv[8]);
    usm_fract             = (float)atof(argv[9]);

    perform_otsu_binarize = atoi(argv[10]);
    otsu_sx               = atoi(argv[11]);
    otsu_sy               = atoi(argv[12]);
    otsu_smoothx          = atoi(argv[13]);
    otsu_smoothy          = atoi(argv[14]);
    otsu_scorefract       = (float)atof(argv[15]);
  }

  /* Read in source image */
  pixs = pixRead(source_file);

  if(pixs == NULL)
  {
    return 1;
  }

  /* Convert to grayscale */
  pixs = pixConvertRGBToGray(pixs, 0.0f, 0.0f, 0.0f);

  if(pixs == NULL)
  {
    return 2;
  }

  if(perform_negate == 1)
  {
    /* Negate image */
    pixInvert(pixs, pixs);

    if(pixs == NULL)
    {
      return 3;
    }
  }
  else if(perform_negate == 2) /* If Auto */
  {
    PIX *otsu_pixs = NULL;
    status = pixOtsuAdaptiveThreshold(pixs, otsu_sx, otsu_sy, otsu_smoothx, otsu_smoothy, otsu_scorefract, NULL, &otsu_pixs);

    if(status != 0)
    {
      return 4;
    }

    /* Get the average intensity of the border pixels */
    border_avg  = pixAverageOnLine(otsu_pixs, 0, 0, otsu_pixs->w - 1, 0, 1);                               /* Top */
    border_avg += pixAverageOnLine(otsu_pixs, 0, otsu_pixs->h - 1, otsu_pixs->w - 1, otsu_pixs->h - 1, 1); /* Bottom */
    border_avg += pixAverageOnLine(otsu_pixs, 0, 0, 0, otsu_pixs->h - 1, 1);                               /* Left */
    border_avg += pixAverageOnLine(otsu_pixs, otsu_pixs->w - 1, 0, otsu_pixs->w - 1, otsu_pixs->h - 1, 1); /* Right */
    border_avg /= 4.0f;

    /* If background is dark */
    if(border_avg > dark_bg_threshold) /* With average of 0.0 being completely white and 1.0 being completely black */
    {
      pixDestroy(&otsu_pixs);

      /* Negate image */
      pixInvert(pixs, pixs);

      if(pixs == NULL)
      {
        return 5;
      }
    }
  }

  if(perform_scale)
  {
    /* Scale the image (linear interpolation) */
    pixs = pixScaleGrayLI(pixs, scale_factor, scale_factor);

    if(pixs == NULL)
    {
      return 6;
    }
  }

  if(perform_unsharp_mask)
  {
    /* Apply unsharp mask */
    pixs = pixUnsharpMaskingGray(pixs, usm_halfwidth, usm_fract);

    if(pixs == NULL)
    {
      return 7;
    }
  }

  if(perform_otsu_binarize)
  {
    /* Binarize */
    status = pixOtsuAdaptiveThreshold(pixs, otsu_sx, otsu_sy, otsu_smoothx, otsu_smoothy, otsu_scorefract, NULL, &pixs);

    if(status != 0)
    {
      return 8;
    }
  }

  /* Get extension of output image */
  status = splitPathAtExtension(dest_file, NULL, &ext);

  if(status != 0)
  {
     return 9;
  }

  /* pixWriteImpliedFormat() doesn't recognize PBM/PGM/PPM extensions */
  if((strcmp(ext, ".pbm") == 0) || (strcmp(ext, ".pgm") == 0) || (strcmp(ext, ".ppm") == 0))
  {
    /* Write the image to file as a PNM */
    status = pixWrite(dest_file, pixs, IFF_PNM);
  }
  else
  {
    /* Write the image to file */
    status = pixWriteImpliedFormat(dest_file, pixs, 0, 0);
  }

  if(status != 0)
  {
    return 10;
  }

  /* Free image data */
  pixDestroy(&pixs);

  return 0;

} /* main */