using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_DrawingBoard
{
    /// <summary>
    /// DrawingBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DrawingBoard : UserControl
    {
        public DrawingBoard()
        {
            InitializeComponent();
        }



        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(Board), typeof(DrawingBoard), new PropertyMetadata());


    }
}
