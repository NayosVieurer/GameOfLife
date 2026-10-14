using GameOfLifeClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GameOfLifeClient.ViewModel
{
    public class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        private int mapWidth = 512;
        private int mapHeight = 512;

        private PipeComm pipeComm;

        public event PropertyChangedEventHandler? PropertyChanged;

        public WriteableBitmap DisplayBitmap { get; private set; }

        public ICommand StartCommand { get; private set; }

        public int RowAmount { get; set; } = 512;
        public int ColAmount { get; set; } = 512;

        public int CellPercentage { get; set; } = 20;

        public MainViewModel()
        {
            pipeComm  = new PipeComm(); // Assuming a grid size of 100 for example
            pipeComm.Connect();

            pipeComm.OnFrameReceived += PipeComm_OnFrameReceived;
            
            StartCommand = new RelayCommand(async () =>
            {
                var startCommand = new Models.StartSimCommand
                {
                    Rows = RowAmount,
                    Cols = ColAmount,
                    ChanceForCell = CellPercentage // 20% chance for a cell to be alive
                };

                pipeComm.GridSize = RowAmount * ColAmount;

                await pipeComm.SendCommandAsync(startCommand);

                mapHeight = RowAmount;
                mapWidth = ColAmount;
                DisplayBitmap = new WriteableBitmap(mapHeight, mapWidth, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayBitmap)));
            });


            //Process.Start("C:\\Users\\Nayos\\source\\repos\\GameOfLife\\x64\\Debug\\GameOfLife.exe");
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
                    DisplayBitmap.AddDirtyRect(new Int32Rect(0, 0, mapHeight, mapWidth));
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

    internal class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action execute;
        private Func<bool> canExecute;

        public RelayCommand(Action exec, Func<bool> can = null) 
        { 
            execute = exec;
            canExecute = can;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute != null ? canExecute() : true;
        }

        public void Execute(object? parameter)
        {
            execute();
        }
    }
}
