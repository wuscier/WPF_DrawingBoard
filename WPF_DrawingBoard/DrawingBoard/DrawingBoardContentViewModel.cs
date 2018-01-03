using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using WPF_DrawingBoard.RedoUndo;

namespace WPF_DrawingBoard
{
    public class DrawingBoardContentViewModel : BindableBase
    {
        private readonly DrawingBoardContentView _drawingBoardView;
        private CommandStack _commandStack;
        private int _editingOperationCount;

        public DrawingBoardContentViewModel(DrawingBoardContentView drawingBoardView)
        {
            InitInkCanvasSettings();

            _drawingBoardView = drawingBoardView;
            _drawingBoardView.Loaded += _drawingBoardView_Loaded;
            StrokesCollection.StrokesChanged += StrokesCollection_StrokesChanged;
            _drawingBoardView.InkCanvas.SelectionMoving += InkCanvas_SelectionResizingOrMoving;
            _drawingBoardView.InkCanvas.SelectionResizing += InkCanvas_SelectionResizingOrMoving;

            GotoSmallBoardCommand = new DelegateCommand(GotoSmallBoard);
            ClearCommand = new DelegateCommand(Clear);
            OpenCommand = new DelegateCommand(Open);
            SaveCommand = new DelegateCommand(Save);
            UndoCommand = new DelegateCommand(Undo);
            RedoCommand = new DelegateCommand(Redo);
        }

        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Ink Serialized Format (*.isf)|*.isf|" +
             "Bitmap files (*.bmp)|*.bmp";

            bool? value = saveFileDialog.ShowDialog();

            if (value.HasValue&&value.Value)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName,FileMode.Create,FileAccess.Write))
                    {
                        if (saveFileDialog.FilterIndex == 1)
                        {
                            _drawingBoardView.InkCanvas.Strokes.Save(fileStream);
                            fileStream.Close();
                        }
                        else
                        {
                            int leftMargin = int.Parse(_drawingBoardView.InkCanvas.Margin.Left.ToString());

                            RenderTargetBitmap renderTargetBitmap =
                                new RenderTargetBitmap((int) _drawingBoardView.ActualWidth - leftMargin,
                                    (int) _drawingBoardView.ActualHeight - leftMargin, 0, 0, PixelFormats.Default);
                            renderTargetBitmap.Render(_drawingBoardView.InkCanvas);
                            BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();
                            bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                            bmpBitmapEncoder.Save(fileStream);
                            fileStream.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Ink Serialized Format (*.isf)|*.isf";

            bool? value = openFileDialog.ShowDialog();

            if (value.HasValue&&value.Value)
            {
                _drawingBoardView.InkCanvas.Strokes.Clear();

                try
                {
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName,FileMode.Open,FileAccess.Read))
                    {
                        if (!openFileDialog.FileName.ToLower().EndsWith(".isf"))
                        {
                            MessageBox.Show("不支持的文件格式！");
                        }
                        else
                        {
                            _drawingBoardView.InkCanvas.Strokes = new StrokeCollection(fileStream);
                            fileStream.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }

        private void Clear()
        {
            _drawingBoardView.InkCanvas.Strokes.Clear();
        }

        private void Redo()
        {
            try
            {
                _commandStack.Redo();
            }
            catch (Exception e)
            {

            }
        }

        private void Undo()
        {
            try
            {
                _commandStack.Undo();
            }
            catch (Exception e)
            {
            }
        }

        private void InkCanvas_SelectionResizingOrMoving(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            Rect newRect = e.NewRectangle;
            Rect oldRect = e.OldRectangle;

            if (newRect.Top < 0d || newRect.Left < 0d)
            {
                Rect newRect2 =
                    new Rect(newRect.Left < 0d ? 0d : newRect.Left,
                        newRect.Top < 0d ? 0d : newRect.Top,
                        newRect.Width,
                        newRect.Height);

                e.NewRectangle = newRect2;
            }
            CommandItem item = new SelectionMovedOrResizedCommandItem(_commandStack,
                _drawingBoardView.InkCanvas.GetSelectedStrokes(), newRect, oldRect, _editingOperationCount);
            _commandStack.Enqueue(item);
        }

        private void StrokesCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            StrokeCollection added = new StrokeCollection(e.Added);
            StrokeCollection removed = new StrokeCollection(e.Removed);

            CommandItem item = new StrokesChangedCommandItem(_commandStack, EditingMode, added, removed,
                _editingOperationCount);
            _commandStack.Enqueue(item);
        }

        private void _drawingBoardView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _commandStack = new CommandStack(StrokesCollection);
        }

        private void GotoSmallBoard()
        {
            _drawingBoardView.InkCanvasContainer.Width = 500;
            _drawingBoardView.InkCanvasContainer.Height = 400;

            Canvas.SetLeft(_drawingBoardView.InkCanvasContainer, 100);
            Canvas.SetTop(_drawingBoardView.InkCanvasContainer, 100);
        }

        private void InitInkCanvasSettings()
        {
            IsPenChecked = true;
            InkColor = Colors.Black;
            

            StrokesCollection = new StrokeCollection();

            DrawingAttributes = new DrawingAttributes()
            {
                Color = InkColor,
                Height = InkThicknessIndex + 2,
                Width = InkThicknessIndex + 2
            };

        }

        private bool _isPenChecked;

        public bool IsPenChecked
        {
            get { return _isPenChecked; }
            set
            {
                if (value)
                {
                    SetProperty(ref _isPenChecked, true);
                    IsEraserChecked = false;
                    IsSelectionChecked = false;
                    IsStrokeEraserChecked = false;
                    EditingMode = InkCanvasEditingMode.Ink;
                }
                else
                {
                    SetProperty(ref _isPenChecked, !(IsEraserChecked || IsSelectionChecked || IsStrokeEraserChecked));
                }
            }
        }


        private bool _isEraserChecked;

        public bool IsEraserChecked
        {
            get { return _isEraserChecked; }
            set
            {
                if (value)
                {
                    SetProperty(ref _isEraserChecked, true);

                    IsPenChecked = false;
                    IsSelectionChecked = false;
                    IsStrokeEraserChecked = false;

                    EditingMode = InkCanvasEditingMode.EraseByPoint;
                    _drawingBoardView.InkCanvas.EraserShape = new RectangleStylusShape(32, 32);
                    //_drawingBoardView.InkCanvas.RenderTransform = new MatrixTransform();

                }
                else
                {
                    SetProperty(ref _isEraserChecked, false);

                    if (!(IsPenChecked || IsStrokeEraserChecked || IsSelectionChecked))
                    {
                        IsPenChecked = true;
                    }
                }
            }
        }

        private bool _isStrokeEraserChecked;

        public bool IsStrokeEraserChecked
        {
            get { return _isStrokeEraserChecked; }
            set
            {
                if (value)
                {
                    SetProperty(ref _isStrokeEraserChecked, true);

                    IsPenChecked = false;
                    IsEraserChecked = false;
                    IsSelectionChecked = false;

                    EditingMode = InkCanvasEditingMode.EraseByStroke;
                }
                else
                {
                    SetProperty(ref _isStrokeEraserChecked, false);

                    if (!(IsPenChecked || IsEraserChecked || IsSelectionChecked))
                    {
                        IsPenChecked = true;
                    }
                }
            }
        }


        private bool _isSelectionChecked;

        public bool IsSelectionChecked
        {
            get { return _isSelectionChecked; }
            set
            {
                if (value)
                {
                    SetProperty(ref _isSelectionChecked, true);

                    IsPenChecked = false;
                    IsEraserChecked = false;
                    IsStrokeEraserChecked = false;

                    EditingMode = InkCanvasEditingMode.Select;
                }
                else
                {
                    SetProperty(ref _isSelectionChecked, false);

                    if (!(IsPenChecked || IsEraserChecked || IsStrokeEraserChecked))
                    {
                        IsPenChecked = true;
                    }
                }
            }
        }


        private InkCanvasEditingMode _editingMode;

        public InkCanvasEditingMode EditingMode
        {
            get { return _editingMode; }
            set { SetProperty(ref _editingMode, value); }
        }

        private DrawingAttributes _drawingAttributes;
        public DrawingAttributes DrawingAttributes
        {
            get { return _drawingAttributes; }
            set { SetProperty(ref _drawingAttributes, value); }
        }

        private int _inkThicknessIndex;

        public int InkThicknessIndex
        {
            get { return _inkThicknessIndex; }
            set
            {
                DrawingAttributes.Height = value + 2;
                DrawingAttributes.Width = value + 2;
                SetProperty(ref _inkThicknessIndex, value);
            }
        }

        private Color _inkColor;
        public Color InkColor
        {
            get { return _inkColor; }
            set
            {
                if (DrawingAttributes != null)
                {
                    DrawingAttributes.Color = value;
                }
                SetProperty(ref _inkColor, value);
            }
        }


        private StrokeCollection _strokeCollection;

        public StrokeCollection StrokesCollection
        {
            get { return _strokeCollection; }
            set { SetProperty(ref _strokeCollection, value); }
        }


        public Color[] ColorPropertiesOdp
        {
            get
            {
                Type colorType = typeof(Colors);
                PropertyInfo[] propertyInfos = colorType.GetProperties();

                List<Color> colors = new List<Color>();

                foreach (var propertyInfo in propertyInfos)
                {
                    Color color = (Color) propertyInfo.GetValue(colorType);
                    colors.Add(color);
                }

                return colors.ToArray();
            }
        }

        public ICommand GotoSmallBoardCommand { get; set; }
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
    }
}