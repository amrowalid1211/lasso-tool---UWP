
# UWP Lasso Tool Example

This project demonstrates how to implement a **Lasso Tool** in a Universal Windows Platform (UWP) application. The lasso tool can be used to select and manipulate areas of an image, such as in a drawing application. This example also includes clipboard integration for copying the selected image area.

## Features

- Drawing a lasso around a portion of an image.
- Highlighting the lassoed area.
- Copying the selected area to the clipboard.
- Removing the lassoed area from the image.

## Technologies Used

- **UWP (Universal Windows Platform)** for creating the application.
- **WriteableBitmap** for image manipulation and drawing.
- **Pointer events** (`PointerPressed`, `PointerMoved`, `PointerReleased`) for tracking lasso drawing.
- **Clipboard API** to copy the selected image to the clipboard.
- **WriteableBitmapExtensions** for image operations like drawing lines and filling polygons.

## How It Works

1. **Pointer Pressed**: Start drawing the lasso when the mouse or touch pointer is pressed on the image.
2. **Pointer Moved**: Track the movement of the pointer and draw the lasso line on the image.
3. **Pointer Released**: Once the pointer is released, capture the lassoed area, manipulate it (clear the lassoed area), and copy the selection to the clipboard.

## Usage

### Step 1: Image Loading
An image is loaded into a `WriteableBitmap` using the `MainPage_Loaded` event. This image serves as the canvas on which the lasso tool will operate.

```csharp
private async void MainPage_Loaded(object sender, RoutedEventArgs e)
{
    imageWriteableBitmap = await BitmapFactory.FromContent(new Uri(this.BaseUri, "/Assets/x.png"));
    lassoImg.Source = imageWriteableBitmap;
}
```

### Step 2: Lasso Drawing
The `PointerPressed`, `PointerMoved`, and `PointerReleased` events handle the drawing of the lasso on the image.

- **PointerPressed**: Initiates the drawing process by setting `isPressed` to `true`.
- **PointerMoved**: Tracks the pointer's movement to draw the lasso line and stores the points.
- **PointerReleased**: Finalizes the lasso selection, clears the lassoed area, and prepares the selected region for further processing (like copying to clipboard).

```csharp
private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
{
    isPressed = true;
}

private void Image_PointerMoved(object sender, PointerRoutedEventArgs e)
{
    var point = e.GetCurrentPoint(sender as Image).Position;
    if (isPressed)
    {
        // Draw line between previous and current pointer position
        imageWriteableBitmap.DrawLine((int)a.X, (int)a.Y, (int)point.X, (int)point.Y, Colors.White);
        lassoImg.Source = imageWriteableBitmap;
        a = point;
    }
}
```

### Step 3: Processing the Lassoed Area
Once the lasso drawing is complete, the selected points are processed. The selected area is cleared, and the content within the lassoed region is copied to the clipboard.

```csharp
private void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
{
    isPressed = false;
    a.X = -1;
    // Process the lassoed area
    clearLassoOnDrawnPoints(visited, c);
}
```

### Step 4: Clipboard Integration
The selected image area is copied to the clipboard in PNG format using `BitmapEncoder`.

```csharp
private async void clearLassoOnDrawnPoints(HashSet<System.Drawing.Point> visited, WriteableBitmap c)
{
    var stream = new InMemoryRandomAccessStream();
    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
    // Prepare image data and set it to the clipboard
    var streamRef = RandomAccessStreamReference.CreateFromStream(stream);
    DataPackage dataPackage = new DataPackage();
    dataPackage.SetBitmap(streamRef);
    Clipboard.SetContent(dataPackage);
}
```

## Running the Project

1. Clone this repository or download the source code.
2. Open the solution in **Visual Studio**.
3. Build the project to restore NuGet packages and dependencies.
4. Run the project on a local machine or a supported UWP device/emulator.

## Assets

- Ensure that the image assets (`x.png` and any other images) are located in the `Assets` folder of the UWP project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Developed by Amro Walid Muhammad Khairy.
