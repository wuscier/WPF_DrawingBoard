using System.Windows.Controls;

namespace WPF_DrawingBoard
{
    /// <summary>
    /// DrawingBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DrawingBoardContentView : UserControl
    {
        public DrawingBoardContentView()
        {
            InitializeComponent();
            DataContext = new DrawingBoardContentViewModel(this);
        }
    }
}
