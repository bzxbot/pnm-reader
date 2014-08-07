using System.Drawing;
using System.IO;

namespace PNMReader
{
    class PNMReader
    {
        public Image ReadImage(string path)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                if (reader.ReadChar() == 'P')
                {
                    char c = reader.ReadChar();

                    if (c == '1')
                    {
                        return ReadTextBitmapImage(reader);
                    }
                    else if (c == '2')
                    {
                        return ReadTextGreyscaleImage(reader);
                    }
                    else if (c == '3')
                    {
                        return ReadTextPixelImage(reader);
                    }
                    else if (c == '6')
                    {
                        return ReadBinaryPixelImage(reader);
                    }
                }
            }

            return null;
        }

        private Image ReadTextBitmapImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int bit = GetNextTextValue(reader) == 0 ? 255 : 0;

                    bitmap.SetPixel(x, y, Color.FromArgb(bit, bit, bit));
                }
            }

            return bitmap;
        }

        private Image ReadTextGreyscaleImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int grey = GetNextTextValue(reader) * 255 / scale;

                    bitmap.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }

            return bitmap;
        }

        private Image ReadTextPixelImage(BinaryReader reader)
        {
            char c;

            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = GetNextTextValue(reader) * 255 / scale;
                    int green = GetNextTextValue(reader) * 255 / scale;
                    int blue = GetNextTextValue(reader) * 255 / scale;

                    bitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return bitmap;
        }

        private Image ReadBinaryBitmapImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int bit = reader.ReadByte() == 0 ? 255 : 0;

                    bitmap.SetPixel(x, y, Color.FromArgb(bit, bit, bit));
                }
            }

            return bitmap;
        }

        private Image ReadBinaryGreyscaleImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int grey = reader.ReadByte() * 255 / scale;

                    bitmap.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }

            return bitmap;
        }

        private Image ReadBinaryPixelImage(BinaryReader reader)
        {
            int width = GetNextHeaderValue(reader);
            int height = GetNextHeaderValue(reader);
            int scale = GetNextHeaderValue(reader);

            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = reader.ReadByte() * 255 / scale;
                    int green = reader.ReadByte() * 255 / scale;
                    int blue = reader.ReadByte() * 255 / scale;

                    bitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return bitmap;
        }

        private int GetNextHeaderValue(BinaryReader reader)
        {
            bool hasValue = false;
            string value = string.Empty;
            char c;
            bool comment = false;

            do
            {
                c = reader.ReadChar();

                if (c == '#')
                {
                    comment = true;
                }

                if (comment)
                {
                    if (c == '\n')
                    {
                        comment = false;
                    }

                    continue;
                }

                if (!hasValue)
                {
                    if ((c == '\n' || c == ' ' || c == '\t') && value.Length != 0)
                    {
                        hasValue = true;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        value += c;
                    }
                }

            } while (!hasValue);

            return int.Parse(value);
        }

        private int GetNextTextValue(BinaryReader reader)
        {
            string value = string.Empty;
            char c = reader.ReadChar();

            do
            {
                value += c;

                c = reader.ReadChar();

            } while (!(c == '\n' || c == ' ' || c == '\t') || value.Length == 0);

            return int.Parse(value);
        }
    }
}