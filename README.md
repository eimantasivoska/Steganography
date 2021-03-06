# Steganography.cs
 
Steganography class which allows you to hide a watermark in an image of the same size and extract it.
Original image and watermark must have color depth of 8 bits. 

This class hides the MSBs (*Most Significant Bits*) of the watermark in the place of the LSBs (*Least Significant Bits*) of the original image.

## Usage
There are two public static methods that were made for interacting with this class.

### Watermarking
This method is used for watermarking the original image with the watermark. Returns a watermarked image.

`public static Bitmap Watermark(Bitmap original, Bitmap watermark, int shift = 4)`

`original` - image to watermark

`watermark` - watermark to hide within the image

`shift` - amount of bits (for red, green and blue for each pixel) that will be preserved. Default amount is **4**.

The higher the `shift` number is, the more bits will be lost from the original image and more bits will be preserved from the 
watermark. Default value of 4 makes sure that both images lose the same amount of information - 4 bits.

### Extracting the watermark
This method is used to extract the watermark from the original image. Returns the watermark.

`public static Bitmap ExtractWatermark(ref Bitmap original, int shift = 4)`

`original` - original image, containing the watermark. This will be cleansed and returned back

`shift` - the amount of LSBs that were used for watermarking that image. Default amount is **4**.
