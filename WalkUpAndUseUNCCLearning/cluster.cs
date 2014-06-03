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
{
    /* This class deals with the members and functions of a cluster. A Cluster comprises of a set of nodes
     */
    public class cluster: Window
    {
        /*Class members
        string name - name of the cluster
        string cid - cluster id e.g., c1
        string nodeColor - color for cluster's nodes
        string textColor - text color for cluster's nodes
        List<node> nodes - List containing node objects for this cluster
        int top - y coordinate where the start of the cluster appears on screen
        int left - x coordinate where the start of the cluster appears on screen
         */
        public string name;
        public string cid;
        public string nodeColor;
        public string textColor;
        public List<node> nodes;
        public int top;
        public int left;

        //Constructor
        public cluster(string Name, string id, string nColor, string tColor, int Top, int Left)
        {
            name = Name;
            cid = id;
            nodeColor = nColor;
            textColor = tColor;
            nodes = new List<node>();
            top = Top;
            left = Left;
        }

        /*This function deletes all the nodes from a cluster
        */
        public void delete()
        {
            nodes.Clear();           
        }

        /*This function adds a node to the cluster
        Parameters 
        node Node- a node object
        */
        public void addNode(node Node)
        {
            nodes.Add(Node);          
        }

        /*This function changes the state of all nodes in a cluster
        Parameters 
        string state- the new state
        */
        public void changeClusterState(string state)
        {
            //assign new state to all nodes inside this cluster
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes.ElementAt(i).state = state;
            }
        }



        /*This function selects a cluster from one of the five clusters
        Parameters 
        Int16[,] zoomedClusProp- Node locations for this selected cluster since they will change their state after getting selected
        */
        public void selectCluster(Int16[,] zoomedClusProp)
        {
            //Change node states for this cluster to "selected"
            changeClusterState("selected");
            //Remove all visual elements from the screen
            MainWindow.PaintCanvas.Children.Clear();
            //Draw the re-cluster information button
            MainWindow.drawButton(0, 1250);
            //Display this selected cluster
            displayCluster(zoomedClusProp, 300, 430, 20,false);

        }


        /*This function creates and displays a node cluster on the screen
        Parameters 
        Int16[,] NodeProp- Node locations
        int top - Top or y coordinate of the cluster location on screen
        int left - Left or x coordinate of the cluster location on screen
        int fontsize - font size of text to be displayed on the nodes of the cluster (only valid for nodes in selected/zoomed states or the title nodes)
        bool zoomed - whether a node is zoomed or not (used for animatin code)
        */
        public void displayCluster(Int16[,] NodeProp, int top, int left, int fontsize,bool zoomed)
        {
            //Due to the layout of a cluster on screen, currently we are only adding the first 7 nodes to a cluster. nCnt now stores the number of nodes inside this cluster.
            int nCnt = nodes.Count < 7 ? nodes.Count : 7;
            //Close all the media (if any) being played in other node
            MainWindow.mp.Stop();
            MainWindow.mp.Close();

            Ellipse ellipse;
            TextBlock txt;
            Grid grid;
            for (int i = 0; i < nCnt; i++)
            {
                 //Create the ellipse to represent the node and display its content
                 ellipse = new Ellipse();
                 ellipse.Height = NodeProp[i, 2];
                 ellipse.Width = NodeProp[i, 2];
                 ellipse.StrokeThickness = 0;
                 BrushConverter bc = new BrushConverter();
                 Brush brush = (Brush)bc.ConvertFrom(nodes.ElementAt(i).color);

                 // store node's name in Textblock
                 txt = new TextBlock();
                 txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                 txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                 txt.TextTrimming = TextTrimming.CharacterEllipsis;

                if (i == 0)
                {
                     txt.FontSize = fontsize + 7;
                    
                }
                else
                {
                     txt.FontSize = fontsize;
                     ellipse.Fill = brush;
                     txt.Margin = new Thickness(15, 10, 15, 10);

                }
                txt.Foreground = (Brush)bc.ConvertFrom(nodes.ElementAt(i).textColor);
                txt.FontWeight = System.Windows.FontWeights.Bold;
                txt.TextWrapping = System.Windows.TextWrapping.Wrap;
                txt.Text = nodes.ElementAt(i).name;
                //Don't display the node name if it is in shrinked or default state. This is because the text will be unreadable
                if ((nodes.ElementAt(i).state == "shrinked" || nodes.ElementAt(i).state == "default") && ellipse.Width<100)
                   txt.Visibility = System.Windows.Visibility.Hidden;

                // Assign values to Labels for storing node's parent ID,
                //node's state, node's name, node's type,node's subject/discipline, and the node's fill color for the ellipse.
                Label lbl = new Label();
                lbl.Content = nodes.ElementAt(i).nodeID;
                lbl.Visibility = System.Windows.Visibility.Hidden;
                Label lbl1 = new Label();
                lbl1.Content = nodes.ElementAt(i).state;
                lbl1.Visibility = System.Windows.Visibility.Hidden;
                Label lbl2 = new Label();
                lbl2.Content = nodes.ElementAt(i).name;
                lbl2.Visibility = System.Windows.Visibility.Hidden;
                Label lbl3 = new Label();
                lbl3.Content = nodes.ElementAt(i).type;
                lbl3.Visibility = System.Windows.Visibility.Hidden;
                Label lbl4 = new Label();
                lbl4.Content = nodes.ElementAt(i).subject;
                lbl4.Visibility = System.Windows.Visibility.Hidden;
                Label lbl5 = new Label();
                lbl5.Content = nodes.ElementAt(i).color;
                lbl5.Visibility = System.Windows.Visibility.Hidden;
                
                //Add the children to the node grid. Each node grid consists of ellipse to show content, Textblock to store textual content, Labels for storing node's parent ID,
                //node's state, node's name, node's type,node's subject/discipline, and the node's fill color for the ellipse.
                grid = new Grid();
                grid.Height = NodeProp[i, 2];
                grid.Width = NodeProp[i, 2];
                grid.Children.Add( ellipse);
                grid.Children.Add( txt);
                grid.Children.Add(lbl);
                grid.Children.Add(lbl1);
                grid.Children.Add(lbl2);
                grid.Children.Add(lbl3);
                grid.Children.Add(lbl4);
                grid.Children.Add(lbl5);
                grid.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow.node_leftClick);
                grid.MouseRightButtonDown += new MouseButtonEventHandler(MainWindow.node_rightClick);
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                //Add this node grid to the Paint Canvas and assign it a location
                MainWindow.PaintCanvas.Children.Add(grid);                
                Canvas.SetLeft( grid, NodeProp[i, 1] + left);
                Canvas.SetTop( grid, NodeProp[i, 0] + top);


                 //this code adds animation on a cluster node by zooming them gradually to the designated size
                 DoubleAnimation da = new DoubleAnimation(1.5, new Duration(TimeSpan.FromSeconds(2)));
                 ScaleTransform sc = new ScaleTransform();
                 grid.RenderTransform = sc;
                 grid.RenderTransformOrigin = new Point(0.5, 0.5);

                 if (!zoomed)
                 {
                     da.AutoReverse = true;
                    
                 }

                 sc.BeginAnimation(ScaleTransform.ScaleXProperty, da);
                 sc.BeginAnimation(ScaleTransform.ScaleYProperty, da);

            }
        }

    }
}