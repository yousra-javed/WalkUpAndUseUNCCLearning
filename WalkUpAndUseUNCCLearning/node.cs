using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
namespace WalkUpAndUseUNCCLearning
{    /* This class deals with the members and functions of a node.
     */
    public partial class node: Window
    {
        /*Class members
        string name  - node name
        string subject - node's discipline e.g., hci
        string type - node's type e.g., course
        string nodeID - node's parent cluster id e.g., c1
        string content - node's content
        string state - node's current state e.g., default 
        string color - node's fill color
        string imgLink - node's image/video path
        string textColor - node's textual content's color
        List<node> relNodes - list of node objects related to this node
        */
        public string name;
        public string subject;
        public string type;
        public string nodeID;
        public string content;
        public string state;
        public string color;
        public string imgLink;
        public string textColor;
        public List<node> relNodes;

        //Constructor
        public node(string nName, string nodeId, string nType, string nContent, string iLink, string nSubject, string State, string nColor, string tColor)  
        {
            name = nName;
            nodeID = nodeId;
            type = nType;
            content = nContent;
            imgLink = iLink;
            subject = nSubject;
            state = State;
            color = nColor;
            textColor = tColor;
            relNodes = new List<node>();
        }

        /*This function displays a node on the screen. It is called only when a node is in the selected state to display its content.
        Parameters 
        Int16[,] NodeProp- Node location
        string textColor - Node's text color
        double top - Top or y coordinate of the cluster location on screen
        double left - Left or x coordinate of the cluster location on screen
        Canvas PaintCanvas - The paintcanvas where it is to be displayed
        int details - this parameter is unused and can be removed
        */
        public void displayNode(Int16[,] NodeProp, string textColor, double top, double left, Canvas PaintCanvas, int details)
        {
                    //stop and close any of the videos playing in previously selected node
                    MainWindow.mp.Stop();
                    MainWindow.mp.Close();
                
                    //Create the ellipse for this node and assign values to its parameters
                    Ellipse ellipse = new Ellipse();
                    ellipse.Height = NodeProp[0, 2];
                    ellipse.Width = NodeProp[0, 2];
                    ellipse.StrokeThickness = 0;
                    BrushConverter bc = new BrushConverter();
                    Brush brush = (Brush)bc.ConvertFrom(color);

                    ImageBrush im = new ImageBrush();
                    Label img = new Label();
                
                    /*This code finds the previously selected node on the paint canvas and removes any related nodes from it and hides its information content 
                     because now another node is being selected for information */
                    foreach (FrameworkElement element in MainWindow.PaintCanvas.Children)
                    {
                        if (element is Grid)
                        {
                            Grid g = element as Grid;
                            if (g != null && g.Children.Count > 6)
                            {
                                Ellipse e = g.Children[0] as Ellipse;
                                Label l = new Label();
                                Label l1 = new Label();
                                l1 = g.Children[6] as Label;
                                Label l2 = g.Children[5] as Label;
                                l = g.Children[7] as Label;
                                Label l3 = g.Children[4] as Label;
                                BrushConverter BC = new BrushConverter();
                                Brush b = (Brush)BC.ConvertFrom(l.Content);
                                TextBlock t = g.Children[1] as TextBlock;
                           
                                if (e.Fill != null && e.Height > 200)
                                {
                               
                                    e.Fill = b;
                                    t.Text = l3.Content.ToString();
                                    t.Visibility = System.Windows.Visibility.Visible;
                                    e.Height = 240;
                                    e.Width = 240;
                               

                                }
                                else if (e.Fill != null && e.Height > 100)
                                {
                                    e.Visibility = System.Windows.Visibility.Hidden;
                                    t.Visibility = System.Windows.Visibility.Hidden;
                                }
                            }


                        }
                    }


                    // store node's name in Textblock
                    TextBlock txt = new TextBlock();
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    txt.FontSize = 20;
                    txt.Foreground = (Brush)bc.ConvertFrom(textColor);
                    txt.FontWeight = System.Windows.FontWeights.Bold;
                    txt.TextWrapping = System.Windows.TextWrapping.Wrap;
                    txt.TextTrimming = TextTrimming.CharacterEllipsis;
                    txt.Margin = new Thickness(15, 12, 15, 10);
                    txt.Text = name;
                    txt.Visibility = System.Windows.Visibility.Hidden;

                    // Assign values to Labels for storing node's parent ID,
                    //node's state, node's name, node's type,node's subject/discipline, and the node's fill color for the ellipse.
                    Label lbl = new Label();
                    lbl.Content = nodeID;
                    lbl.Visibility = System.Windows.Visibility.Hidden;
                    Label lbl1 = new Label();
                    lbl1.Content = state;
                    lbl1.Visibility = System.Windows.Visibility.Hidden;
                    Label lbl2 = new Label();
                    lbl2.Content = name;
                    lbl2.Visibility = System.Windows.Visibility.Hidden;
                    Label lbl3 = new Label();
                    lbl3.Content = type;
                    lbl3.Visibility = System.Windows.Visibility.Hidden;
                    Label lbl4 = new Label();
                    lbl4.Content = subject;
                    lbl4.Visibility = System.Windows.Visibility.Hidden;
                    Label lbl5 = new Label();
                    lbl5.Content = color;
                    lbl5.Visibility = System.Windows.Visibility.Hidden;

                    //Add the children to the node grid. Each node grid consists of ellipse to show content, Textblock to store textual content, Labels for storing node's parent ID,
                    //node's state, node's name, node's type,node's subject/discipline, and the node's fill color for the ellipse.
                    Grid grid = new Grid();
                    grid.Height = NodeProp[0, 2];
                    grid.Width = NodeProp[0, 2];
                    grid.Children.Add(ellipse);
                    grid.Children.Add(txt);
                    grid.Children.Add(lbl);
                    grid.Children.Add(lbl1);
                    grid.Children.Add(lbl2);
                    grid.Children.Add(lbl3);
                    grid.Children.Add(lbl4);
                    grid.Children.Add(lbl5);
                    
                    grid.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow.node_leftClick);

                    //if this node has a video or image content, get its path and play/display it
                    if (imgLink != "")
                    {
                        MainWindow.mp.Open(new Uri(@imgLink, UriKind.Relative));
                        VideoDrawing vDrawing = new VideoDrawing();
                        vDrawing.Rect = new Rect(0, 0, 100, 100);
                        vDrawing.Player = MainWindow.mp;
                        DrawingBrush db = new DrawingBrush(vDrawing);
                        ellipse.Fill = db;
                        MainWindow.mp.Play();

                    }
                    //if this node does not have a video or image content, display its text information
                    else
                    {
                        txt.Visibility = System.Windows.Visibility.Visible;
                        txt.Text = content;
                        txt.FontSize = 13;
                        BrushConverter BC = new BrushConverter();
                        Brush b = (Brush)BC.ConvertFrom(color);
                        ellipse.Fill = b;  
                    
                    }
             

                    PaintCanvas.Children.Add(grid);


                    //this code animates the node and makes it get larger gradually when selected
                    DoubleAnimation da = new DoubleAnimation(1.1, new Duration(TimeSpan.FromSeconds(2)));

                    ScaleTransform sc = new ScaleTransform();
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(sc);//add scaleTransform

                    grid.RenderTransform = tg;
                    grid.RenderTransformOrigin = new Point(0, 0);
                    sc.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                    sc.BeginAnimation(ScaleTransform.ScaleYProperty, da);

                    Canvas.SetLeft(grid, NodeProp[0, 1] + left);
                    Canvas.SetTop(grid, NodeProp[0, 0] + top);

                    // this loop deals with displaying all the related nodes of this particular node
                    for (int i = 0; i < relNodes.Count; i++)
                    {

                        ellipse = new Ellipse();
                        ellipse.Height = NodeProp[i + 1, 2];
                        ellipse.Width = NodeProp[i + 1, 2];
                        ellipse.StrokeThickness = 1.5;
                        bc = new BrushConverter();
                        brush = (Brush)bc.ConvertFrom(relNodes.ElementAt(i).color);
                        ellipse.Fill = brush;

                        txt = new TextBlock();
                        txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        txt.FontSize = 16;
                        txt.Foreground = (Brush)bc.ConvertFrom(textColor);
                        txt.FontWeight = System.Windows.FontWeights.Bold;
                        txt.TextWrapping = System.Windows.TextWrapping.Wrap;
                        txt.TextTrimming = TextTrimming.CharacterEllipsis;
                        txt.Margin = new Thickness(15, 12, 15, 10);
                        txt.Text = name;

                        lbl = new Label();
                        lbl.Content = relNodes.ElementAt(i).nodeID;
                        lbl.Visibility = System.Windows.Visibility.Hidden;
                        lbl1 = new Label();
                        lbl1.Content = relNodes.ElementAt(i).state;
                        lbl1.Visibility = System.Windows.Visibility.Hidden;
                        lbl2 = new Label();
                        lbl2.Content = relNodes.ElementAt(i).name;
                        lbl2.Visibility = System.Windows.Visibility.Hidden;
                        lbl3 = new Label();
                        lbl3.Content = relNodes.ElementAt(i).type;
                        lbl3.Visibility = System.Windows.Visibility.Hidden;
                        lbl4 = new Label();
                        lbl4.Content = subject;
                        lbl4.Visibility = System.Windows.Visibility.Hidden;
                        lbl5 = new Label();
                        lbl5.Content = color;
                        lbl5.Visibility = System.Windows.Visibility.Hidden;
                        txt.Text = relNodes.ElementAt(i).name;


                        grid = new Grid();
                        grid.Height = NodeProp[i + 1, 2];
                        grid.Width = NodeProp[i + 1, 2];
                        grid.Children.Add(ellipse);
                        grid.Children.Add(txt);
                        grid.Children.Add(lbl);
                        grid.Children.Add(lbl1);
                        grid.Children.Add(lbl2);
                        grid.Children.Add(lbl3);
                        grid.Children.Add(lbl4);
                        grid.Children.Add(lbl5);
                        PaintCanvas.Children.Add(grid);
                        Canvas.SetLeft(grid, 0);
                        Canvas.SetTop(grid, 0);
                        
                        // this code moves the related nodes from the top left corner of the screen onto the selected node
                        DoubleAnimationUsingPath doublePathLR = new DoubleAnimationUsingPath();
                        DoubleAnimationUsingPath doublePathUD = new DoubleAnimationUsingPath();
                        TranslateTransform tr = new TranslateTransform();
                        grid.RenderTransform = tr;
                        PathGeometry pg = new PathGeometry();
                        PathFigure pFigure = new PathFigure();           
                        LineSegment lSegment = new LineSegment();
                    

                        PathSegmentCollection collection = new PathSegmentCollection();
                        collection.Add(lSegment);
                        lSegment.Point = new Point(NodeProp[i + 1, 1] + left, NodeProp[i + 1, 0] + top);//(200, 70);
                        collection.Add(lSegment);
                        pFigure.Segments = collection;
                        PathFigureCollection conn = new PathFigureCollection();
                        conn.Add(pFigure);
                        pg.Figures = conn;

                        doublePathLR.PathGeometry = pg;
                        doublePathLR.Duration = TimeSpan.FromSeconds(3);
                        doublePathLR.Source = PathAnimationSource.X;
                        doublePathUD.PathGeometry = pg;
                        doublePathUD.Duration = TimeSpan.FromSeconds(3);
                        doublePathUD.Source = PathAnimationSource.Y;

                        //this actually makes the node move; it activates the animation
                        tr.BeginAnimation(TranslateTransform.XProperty, doublePathLR);
                        tr.BeginAnimation(TranslateTransform.YProperty, doublePathUD);
                        



                    }

           
        }


    }
}