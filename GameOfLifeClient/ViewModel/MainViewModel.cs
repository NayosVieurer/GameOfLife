using GameOfLifeClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GameOfLifeClient.ViewModel
{
    public class MainViewModel : IDisposable
    {
        private PipeComm pipeComm;

        public WriteableBitmap DisplayBitmap { get; private set; }

        public MainViewModel()
        {
            pipeComm  = new PipeComm(); // Assuming a grid size of 100 for example
            pipeComm.Connect();

            pipeComm.OnFrameReceived += PipeComm_OnFrameReceived;

            DisplayBitmap = new WriteableBitmap(512, 512, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
        }

        private void PipeComm_OnFrameReceived(object? sender, byte[] buffer)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (DisplayBitmap == null) return;

                DisplayBitmap.Lock();
                try
                {
                    IntPtr backBuffer = DisplayBitmap.BackBuffer;

                    // Copie matérielle ultra-rapide (Zéro CPU) du buffer managé vers la Bitmap WPF
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, backBuffer, buffer.Length);

                    // On notifie WPF qu'il faut rafraîchir l'écran sur cette zone
                    DisplayBitmap.AddDirtyRect(new Int32Rect(0, 0, 512, 512));
                }
                finally
                {
                    DisplayBitmap.Unlock();
                }
            });                  
        }

        public void Dispose()
        {
            DisplayBitmap = null;

            if(pipeComm != null)
                pipeComm.OnFrameReceived -= PipeComm_OnFrameReceived;
        }
    }
}
