using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Sprite_Splitter
{
    public class SpriteReader
    {
        public Color transparentColor = Color.Black;
        public List<List<Bitmap>> Images = new List<List<Bitmap>>();
        public float spriteWidth, spriteHeight;

        public void GetTransparentColor(Bitmap image, int x, int y)
        {
            transparentColor = image.GetPixel(x, y);
        }

        public void ReadImage(Bitmap image, float maxReadX, float maxReadY)
        {
            image.MakeTransparent(transparentColor);
            float w = image.Width % spriteWidth;
            if (maxReadX > 0f) w = maxReadX;
            float wi = 0;
            float h = image.Height % spriteHeight;
            if (maxReadY > 0f) h = maxReadY;
            float hi = 0;
            while (hi < h)
            {
                if (Images.Count == hi) Images.Add(new List<Bitmap>());
                while (wi < w)
                {
                    Images[(int)hi].Add(image.Clone(new Rectangle((int)(wi * spriteWidth), (int)(hi * spriteHeight), (int)spriteWidth, (int)spriteHeight), PixelFormat.Format32bppArgb));
                    wi++;
                }
                hi++;
            }
        }

        public Bitmap CombineXImages(int spriteIndex, params int[] index)
        {
            Bitmap bmp = new Bitmap((int)(spriteWidth * index.Length), (int)spriteHeight);
            Graphics grp = Graphics.FromImage(bmp);
            for (int i = 0; i < index.Length; i++)
            {
                grp.DrawImage(Images[spriteIndex][index[i]], i * spriteWidth, spriteHeight);
            }
            return bmp;
        }

        public Bitmap CombineYImages(int spriteIndex, params int[] index)
        {
            Bitmap bmp = new Bitmap((int)spriteWidth, (int)(spriteHeight * index.Length));
            Graphics grp = Graphics.FromImage(bmp);
            for (int i = 0; i < index.Length; i++)
            {
                grp.DrawImage(Images[index[i]][spriteIndex], spriteWidth, i * spriteHeight);
            }
            return bmp;
        }

        public Rectangle[] findRectsSprites(Bitmap image, Point start, Size scanSize)
        {
            image.MakeTransparent(transparentColor);
            List<Rectangle> rects = new List<Rectangle>();
            Rectangle lastRectangle = new Rectangle(start, new Size(0, 0));
            int bigheight = 0;

            while (true)
            {
                try
                {
                    Rectangle temprect = findImageFinalRect(image, lastRectangle.Location, scanSize);

                    if (temprect.Width > 1 && temprect.Height > 1)
                    {
                        rects.Add(temprect);
                        lastRectangle.X = temprect.X;
                        lastRectangle.Width = temprect.Width;
                        lastRectangle.Height = temprect.Height;
                        if (temprect.Height > bigheight)
                        {
                            bigheight = (temprect.Y - lastRectangle.Y) + temprect.Height;
                        }

                        if ((lastRectangle.X + lastRectangle.Width) < image.Width)
                        {
                            lastRectangle.X += lastRectangle.Width + 1;
                        }
                    }
                    else if (temprect.Width > 0 && temprect.Width < 5 || temprect.Height > 0 && temprect.Height < 5)
                    {
                        lastRectangle.X += temprect.Width;
                    }
                    else if ((lastRectangle.Y + bigheight) > 1 && (lastRectangle.Y + bigheight) < image.Height)
                    {
                        if (bigheight == 0) break;
                        lastRectangle.X = start.X;
                        lastRectangle.Y += bigheight + 1;
                        bigheight = 0;
                    }
                    else break;
                }
                catch { break; }
            }
            return rects.ToArray();
        }

        public void findSprites(Bitmap image, Point start, Size scanSize)
        {
            image.MakeTransparent(transparentColor);
            Rectangle lastRectangle = new Rectangle(start, new Size(0, 0));
            int i = 0, bigheight = 0;
            while (true)
            {
                try
                {
                    Rectangle temprect = findImageFinalRect(image, lastRectangle.Location, scanSize);

                    if (temprect.Width > 1 && temprect.Height > 1)
                    {
                        if (Images.Count == i) Images.Add(new List<Bitmap>());
                        Images[i].Add(image.Clone(temprect, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
                        lastRectangle.X = temprect.X;
                        lastRectangle.Width = temprect.Width;
                        lastRectangle.Height = temprect.Height;
                        if (temprect.Height > bigheight)
                        {
                            bigheight = (temprect.Y - lastRectangle.Y) + temprect.Height;
                        }

                        if ((lastRectangle.X + lastRectangle.Width) < image.Width)
                        {
                            lastRectangle.X += lastRectangle.Width + 1;
                        }
                    }
                    else if (temprect.Width != 0 && temprect.Width < 5 || temprect.Height != 0 && temprect.Height < 5)
                    {
                        lastRectangle.X += temprect.Width;
                    }
                    else if ((lastRectangle.Y + bigheight) > 1 && (lastRectangle.Y + bigheight) < image.Height)
                    {
                        if (bigheight == 0) break;
                        lastRectangle.X = start.X;
                        lastRectangle.Y += bigheight + 1;
                        i++;
                        bigheight = 0;
                    }
                    else break;
                }
                catch { break; }
            }
        }

        private Rectangle findImageFinalRect(Bitmap image, Point start, Size scanSize)
        {
            Rectangle rect = Rectangle.Empty;
            while (true)
            {
                Rectangle temprect = findImageRect(image, start, scanSize);
                if (rect == temprect)
                {
                    break;
                }
                else
                {
                    if (temprect.Width < 0 || temprect.Height < 0) break;
                    rect = temprect;
                }
            }
            return rect;
        }

        private Rectangle findImageRect(Bitmap image, Point start,Size scanSize)
        {
            int ix = start.X, width = 0, iy = start.Y, height = 0;
            int first = 0;
            bool next = false;
            int scanwidth = image.Width, scanheight = image.Height;
            if ((start.X + scanSize.Width) < image.Width) scanwidth = (start.X + scanSize.Width);
            if ((start.Y + scanSize.Height) < image.Height) scanheight = (start.Y + scanSize.Height);

            for (int x = start.X; x < scanwidth; x++)
            {
                for (int y = start.Y; y < scanheight; y++)
                {
                    Color clr = image.GetPixel(x, y);
                    if (clr == Color.FromArgb(0, 0, 0, 0))
                    {
                        next = true;
                    }
                    else
                    {
                        next = false;
                        break;
                    }
                }
                if (next)
                {
                    if (first == 0)
                        ix = x;
                    else if (first == 1)
                        width = x;
                }
                else
                {
                    first = 1;
                }
                if (first == 1 && width > 0) break;
            }
            if (ix == width) return Rectangle.Empty;
            start.X = ix;
            next = false;
            first = 0;
            for (int y = start.Y; y < scanheight; y++)
            {
                for (int x = start.X; x < width; x++)
                {
                    Color clr = image.GetPixel(x, y);
                    if (clr == Color.FromArgb(0, 0, 0, 0))
                    {
                        next = true;
                    }
                    else
                    {
                        next = false;
                        break;
                    }
                }
                if (next)
                {
                    if (first == 0)
                        iy = y;
                    else if (first == 1)
                        height = y;
                }
                else
                {
                    first = 1;
                }
                if (first == 1 && height > 0) break;
            }
            if (iy == height) return Rectangle.Empty;
            return new Rectangle(ix, iy, width - ix, height - iy);
        }
    }
}
