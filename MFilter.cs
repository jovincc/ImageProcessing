﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Numerics;

public static class MFilter
{
    public static Bitmap MedianFilter(this Bitmap sourceBitmap,int matrixSize,int bias = 0,bool grayscale = false)
    {
        BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
        byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
        sourceBitmap.UnlockBits(sourceData);

        if (grayscale == true)
        {
            float rgb = 0;

            for (int k = 0; k < pixelBuffer.Length; k += 4)
            {
                rgb = pixelBuffer[k] * 0.11f;
                rgb += pixelBuffer[k + 1] * 0.59f;
                rgb += pixelBuffer[k + 2] * 0.3f;

                pixelBuffer[k] = (byte)rgb;
                pixelBuffer[k + 1] = pixelBuffer[k];
                pixelBuffer[k + 2] = pixelBuffer[k];
                pixelBuffer[k + 3] = 255;
            }
        }


        int filterOffset = (matrixSize - 1) / 2;
        int calcOffset = 0;


        int byteOffset = 0;

        List<int> neighbourPixels = new List<int>();
        byte[] middlePixel;


        for (int offsetY = filterOffset; offsetY <
            sourceBitmap.Height - filterOffset; offsetY++)
        {
            for (int offsetX = filterOffset; offsetX <
                sourceBitmap.Width - filterOffset; offsetX++)
            {
                byteOffset = offsetY *
                             sourceData.Stride +
                             offsetX * 4;


                neighbourPixels.Clear();


                for (int filterY = -filterOffset;
                    filterY <= filterOffset; filterY++)
                {
                    for (int filterX = -filterOffset;
                        filterX <= filterOffset; filterX++)
                    {


                        calcOffset = byteOffset +
                                     (filterX * 4) +
                            (filterY * sourceData.Stride);


                        neighbourPixels.Add(BitConverter.ToInt32(
                                         pixelBuffer, calcOffset));
                    }
                }


                neighbourPixels.Sort();

                middlePixel = BitConverter.GetBytes(
                                   neighbourPixels[filterOffset]);


                resultBuffer[byteOffset] = middlePixel[0];
                resultBuffer[byteOffset + 1] = middlePixel[1];
                resultBuffer[byteOffset + 2] = middlePixel[2];
                resultBuffer[byteOffset + 3] = middlePixel[3];
            }
        }


        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
        BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

        resultBitmap.UnlockBits(resultData);


        return resultBitmap;
    }

}
    //public static Bitmap lowpass_filter(Bitmap lpf_img)
    //{
    //    int sumr, sumg, sumb;
    //    sumr = sumg = sumb = 0;
    //    int idk;
    //    int blk = 3;
    //    for (int y = 0; y < lpf_img.Height-blk; y++)
    //    {
    //        for (int x = 0; x < lpf_img.Width-blk; x++)
    //        {
    //            for (int i = 0; i < blk; i++)
    //                for (int j = 0; j < blk; j++)
    //                {

    //                    Color color = lpf_img.GetPixel(x, y);
    //                    sumr += idk[i + x][j + y].r;
    //                    sumg += lpf_img[i + x][j + y].g;
    //                    sumb += lpf_img[i + x][j + y].b;
    //                }
    //            bitmap[i][j].r = sumr / (blk * blk);
    //            bitmap[i][j].g = sumg / (blk * blk);
    //            bitmap[i][j].b = sumb / (blk * blk);
    //            sumr = sumg = sumb = 0;
    //            int gray = (byte)((color.R * 0.299) + (color.G * 0.114) + (color.B * 0.587));
    //            lpf_img.SetPixel(x, y, Color.FromArgb(255 - gray, 255 - gray, 255 - gray));
    //        }
    //    }
    //    return lpf_img;
    //}


/*read_img();
CDC *pdc=GetDC();
for(i=0;i<heit-blk;i++)
for(j=0;j<width-blk;j++)
{
for(x=0;x<blk;x++)
for(y=0;y<blk;y++)
{

}
bitmap[i][j].r=sumr/(blk*blk);
bitmap[i][j].g=sumg/(blk*blk);
bitmap[i][j].b=sumb/(blk*blk);
sumr=sumg=sumb=0;
}
for(i=0;i<heit;i++)
for(j=0;j<width;j++)
pdc->SetPixel(i,j,RGB(bitmap[i][j].r,bitmap[i][j].g,
bitmap[i][j].b));*/