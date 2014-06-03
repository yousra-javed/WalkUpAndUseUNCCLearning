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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Microsoft.Kinect;
using System.IO;

namespace WalkUpAndUseUNCCLearning
{

    public partial class MainWindow : Window
    {
        //Initialize variables
        public static int counter = 1; //This counter is responsible for changing the re-clustering the nodes by type or discipline
        public static Random rnd = new Random(); //Random number generation
        public static System.Windows.Threading.DispatcherTimer timer; //Timer based upon which the counter is incremented
        public static System.Windows.Threading.DispatcherTimer t;
        public static int activestate =0;//This variable indicates whether the system is in active state or not
        public static MediaPlayer mp = new MediaPlayer(); //The mediaplayer plays a video clip
        /*
        There are five static cluster objects in the system. The nodes for these clusters change dynamically based on the clustering criteria
        When the system starts, the following five empty clusters are created
        c1 - Security cluster
        c2 - HCI cluster
        C3 - Software systems cluster
        c4 - Health Informatics cluster
        c5 - Intelligent Information systems cluster
         */
        public static cluster c1 = new cluster("Security", "c1", "#6C4C77", "Black", 80, 80);
        public static cluster c2 = new cluster("HCI", "c2", "#C13045", "Black",550, 1150);
        public static cluster c3 = new cluster("SW", "c3", "#2d3f51", "Black", 550, 80);
        public static cluster c4 = new cluster("HCIP", "c4", "#19978A", "Black",60, 1150);
        public static cluster c5 = new cluster("IIS", "c5", "#ED9A33", "Black", 440, 650);
        
        /*
        This array stores - at each index - the {top location,left location,size} of a node of cluster when it is in default mode i.e.,
        right before the user selects any cluster. The values at first index of this array represent first node's location and size,
        the values at second index represent the second node's location and size, and so on for all the nodes in a cluster.
        Then in the display cluster function, for each cluster an additional offset is added to these locations to move them to the top left, top right, bottem left,
        bottom right corners, and middle of the screen
        */ 
        public static Int16[,] defNodeProp = { { 70, 190, 150 }, { 81, 82, 90 }, { 14, 135, 90 }, { 3, 47, 90 }, { 75, -5, 90 }, { 155, 30, 90 }, { 160, 118, 90 }, { 145, -55, 90 }, { -5, -40, 90 }, { 145, -55, 90 }, { 145, -55, 90 }, { 145, -55, 90 }, { 145, -55, 90 } };
        /*
        This array stores - at each index - the {top location,left location,size} of a node of cluster when the user selects a cluster and it is zoomed.
        The values at first index of this array represent first node's location and size,
        the values at second index represent the second node's location and size, and so on for all the nodes in the selected cluster.
        There can only be one selected cluster at a time.
        */ 
        public static Int16[,] zoomedClusProp = { { 80, 260, 180 }, { -190, 390, 240 }, { -190, 50, 240 }, { 50, -90, 240 }, { 280, 50, 240 }, { 280, 390, 240 }, { 50, 580, 240 }, { -90, 80, 240 }, { -90, 80, 200 }, { -90, 80, 200 }, { -90, 80, 200 }, { -90, 80, 200 }, { -90, 80, 200 } };
        /*
        This array stores - at each index - the {top location,left location,size} of a node and its related nodes when a node is selected
        from within the selected cluster
        The values at first index of this array represent selected node's location and size, the values at second index represent
        the location and size for the first related node of the selected node, the values at the third index represent the location and size
        for the second related node of the selected node and so on for all the related nodes in the selected cluster.
        There can only be one zoomed node at a time.
        */ 
        public static Int16[,] zoomedNodeProp = { { -40, -40, 240 }, { -145,-80, 130 }, { -170, 69, 130 }, { -80, 190, 130 }, { 62, 200, 130 }, { 100, 290, 130 }, { 182, 470, 130 }, { 129, 164, 105 }, { 129, 164, 105 } };
        
         /*
        This array stores - at each index - the {top location,left location,size} of a node of cluster when it is in shrinked mode i.e.,
        after the user selects any cluster, all other clusters are shrinked. The values at first index of this array represent first node's location and size,
        the values at second index represent the second node's location and size, and so on for all the nodes in a cluster.
        Then in the display cluster function, for each shrinked cluster an additional offset is added to these locations to move them to the top left, top right, bottem left,
        and bottom right corners. The middle of the screen displays the selected cluster
        */        
        public static Int16[,] shrinkNodeProp = { { 60, 200, 120 }, { 70, 95, 50 }, { 80, 150, 50 }, { 115, 120, 50 }, { 140, 165, 50 }, { 125, 70, 50 }, { 160, 110, 50 }, { 79, 40, 55 }, { 79, 40, 55 }, { 79, 40, 55 }, { 79, 40, 55 }, { 79, 40, 55 }, { 79, 40, 55 }, { 79, 40, 55 } };
        
        public static List<node> Nodes = new List<node>(); //List data structure to store the nodes loaded from the database
        public static Canvas PaintCanvas = new Canvas(); //The canvas on which all visual elements are displayed
        public static Skeleton skel; //This variable stores the skeleton joint data of the user in front of the system
        public static string[] subtype; //The string array stores the node types or disciplines based on which the nodes are clustered
        public static gestureProcessor gp= new gestureProcessor(); // This object detects and processes the gestures
        private KinectSensor sensor; //Kinect sensor attached to the system
        
        
        public MainWindow()
        {

            InitializeComponent();
            gp = new gestureProcessor();
            
            /*This code creates the paint canvas, adjusts its size to that of the underlying display size and
            adds the grey background color to it */
            Color c = Color.FromRgb(108, 108, 108);
            SolidColorBrush s = new SolidColorBrush();
            s.Color = c;
            name.Background = s;
            name.Width = SystemParameters.PrimaryScreenWidth;
            PaintCanvas.Background = s;
            PaintCanvas.Height = SystemParameters.PrimaryScreenHeight;
            PaintCanvas.Width = SystemParameters.PrimaryScreenWidth;
            UNCCLearning.Content = PaintCanvas;
            
            //Load node data from the database
            loadData data = new loadData();
            
            //Group the nodes by discipline, and assign them to the five cluster objects
            subtype = new string[5] { "sec", "hci", "sw", "hcip", "iis" };
            reorderClusters(subtype);
            
            //Draw the re-cluster information button on the screen
            MainWindow.drawButton(120, 950);
            
            //Set a 5 second timer
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); //Set the interval period here.
            timer.Tick += new EventHandler(timer_Tick);// After every 5 seconds this event handler is called
            timer.Start();
            
         
        }

        /*Recluster the nodes by type or discipline alternatively in the background after every 5 seconds. So, when the system switches to active
        mode, either one is displayed*/
        void timer_Tick(object sender, EventArgs e)
        {
            //delete the existing nodes of the clusters
            c1.delete();
            c2.delete();
            c3.delete();
            c4.delete();
            c5.delete();
            
            if (counter % 2 == 0)  // If counter is even, recluster by type
            {
                subtype = new string[5] { "club", "research", "course", "faculty", "event" }; 
                reorderClusters(subtype);                
                
            }
            else // If counter is odd, recluster by discipline
            {
                subtype = new string[5] { "sec", "hci", "sw", "hcip", "iis" };
                reorderClusters(subtype);

            }

            counter += 1; //increment the counter
           
        }

        /*Recluster the nodes by type or discipline by selecting the "change information layout" button */       
        public static void changeClusterReordering()
        {
                //delete the existing nodes of the clusters
                c1.delete();
                c2.delete();
                c3.delete();
                c4.delete();
                c5.delete();
                
                if (subtype[0]=="sec")  // If the nodes are currently clustered by discipline, cluster them by type
                {
                    subtype = new string[5] { "club", "research", "course", "faculty", "event" };
                    reorderClusters(subtype);

                }
                else if (subtype[0] == "club") // If the nodes are currently clustered by type, cluster them by discipline
                {
                    subtype = new string[5] { "sec", "hci", "sw", "hcip", "iis" };
                    reorderClusters(subtype);

                }
                
                //all the nodes should now be in default mode
                c1.changeClusterState("default");
                c2.changeClusterState("default");
                c3.changeClusterState("default");
                c4.changeClusterState("default");
                c5.changeClusterState("default");
                
                //remove all the existing visual elements from the paint canvas
                MainWindow.PaintCanvas.Children.Clear();
                
                //display the new clusters on the paint canvas
                MainWindow.c1.displayCluster(defNodeProp, 100, 80, 20, false);
                MainWindow.c2.displayCluster(defNodeProp, 550, 1150, 20, false);
                MainWindow.c3.displayCluster(defNodeProp, 550, 80, 20, false);
                MainWindow.c4.displayCluster(defNodeProp, 100, 1150, 20, false);
                MainWindow.c5.displayCluster(defNodeProp, 440, 650, 20, false);
                activestate = 1;// system is still active
                gestureProcessor.RframeCount = 0; // reset the right hand frame count
                gestureProcessor.LframeCount = 0; // reset the left hand frame count
                drawButton(120, 700); // redraw the "change information layout" button
            

        }
        
        //This function draws the "change information layout" button on the screen that is used to re-cluster the information nodes
        public static void drawButton(int top, int left)
        {
                        //create a visual grid, add an ellipse to create round button, and textblock to add the button text
                        Grid gr = new Grid();
                        gr.Height = 150;
                        gr.Width = 150;
                        Label l = new Label();
                        TextBlock t = new TextBlock();
                        Ellipse ell = new Ellipse();
                        ell.Width = 140;
                        ell.Height = 140;
                        ell.Stroke = new SolidColorBrush(Colors.White);
                        ell.StrokeThickness = 0;
                        BrushConverter bc = new BrushConverter();
                        ell.Fill = (Brush)bc.ConvertFrom("Black");
                        ell.Visibility = System.Windows.Visibility.Visible;
                        
                        gr.Visibility = System.Windows.Visibility.Visible;
                        gr.Children.Add(ell);
                        l.Content = "button";
                        l.Visibility = System.Windows.Visibility.Hidden;
                        
                        t.FontSize = 22;
                        t.Foreground = (Brush)bc.ConvertFrom("White");
                        t.Margin = new Thickness(14, 25, 10, 10);
                        t.Text = "Switch Information Layout"; 
                        t.TextTrimming = TextTrimming.CharacterEllipsis;
                        t.FontWeight = System.Windows.FontWeights.Bold;
                        t.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        t.TextAlignment = TextAlignment.Center;
                        t.TextWrapping = System.Windows.TextWrapping.Wrap;
                        t.Visibility = System.Windows.Visibility.Visible;
                        gr.Children.Add(t);
                        gr.Children.Add(l);
                        l = new Label();
                        l.Visibility = System.Windows.Visibility.Hidden;
                        l.Content = "button";
                        gr.Children.Add(l);
                        l = new Label();
                        l.Visibility = System.Windows.Visibility.Hidden;
                        l.Content = "button";
                        gr.Children.Add(l);
                        
                        //Add it to the paint canvas and set a location
                        MainWindow.PaintCanvas.Children.Add(gr);
                        Canvas.SetTop(gr, top);
                        Canvas.SetLeft(gr, left);
        }
        
        /*
        This function adds the relevant nodes to the relevant clusters
         */
        public static void reorderClusters(string[] SubType)
        {
           
            //for all the nodes
            for (int i = 0; i < Nodes.Count; i++)
            {
                //check if the node has type equal to club or discipline equal to security, assign it to cluster c1
                if (Nodes.ElementAt(i).subject == SubType[0] || Nodes.ElementAt(i).type == SubType[0])
                {
                    
                    Nodes.ElementAt(i).nodeID = "c1";
                    Nodes.ElementAt(i).state= "default";
                    c1.addNode(Nodes.ElementAt(i));
                }
                //check if the node has type equal to research or discipline equal to hci, assign it to cluster c2
                else if (Nodes.ElementAt(i).subject == SubType[1] || Nodes.ElementAt(i).type == SubType[1])
                {
                    Nodes.ElementAt(i).nodeID = "c2";
                    Nodes.ElementAt(i).state = "default";
                    c2.addNode(Nodes.ElementAt(i));
                }
                //check if the node has type equal to course or discipline equal to software systems, assign it to cluster c3
                else if (Nodes.ElementAt(i).subject == SubType[2] || Nodes.ElementAt(i).type == SubType[2])
                {
                    Nodes.ElementAt(i).nodeID = "c3";
                    Nodes.ElementAt(i).state = "default";
                    c3.addNode(Nodes.ElementAt(i));
                }
                //check if the node has type equal to faculty or discipline equal to health informatics, assign it to cluster c4
                else if (Nodes.ElementAt(i).subject == SubType[3] || Nodes.ElementAt(i).type == SubType[3])
                {
                    Nodes.ElementAt(i).nodeID = "c4";
                    Nodes.ElementAt(i).state = "default";
                    c4.addNode(Nodes.ElementAt(i));
                }
                //check if the node has type equal to event or discipline equal to intelligent information systems, assign it to cluster c5
                else if (Nodes.ElementAt(i).subject == SubType[4] || Nodes.ElementAt(i).type == SubType[4])
                {
                    Nodes.ElementAt(i).nodeID = "c5";
                    Nodes.ElementAt(i).state = "default";
                    c5.addNode(Nodes.ElementAt(i));
                }
            }
            
        }

        //This function is executed when the system is started
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //check for any connected Kinect sensor
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            
            if (null != this.sensor)
            {
                // Turn on the color and skeleton stream to receive user's video and skeleton frames
                
                this.sensor.ColorStream.Enable();
                this.sensor.SkeletonStream.Enable();
                
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.ColorFrameReady;
                // Add an event handler to be called whenever there is new skeleton frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                
                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {

            }
            
        }
        
        //global variables related to displaying color frame data
        byte[] colorData = null;
        IntPtr colorPtr;
        Image im;
        /*
         This function deals with displaying a colorframe on the screen as soon as it is ready
         */
        private void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) return;

                if (colorData == null)
                    colorData = new byte[colorFrame.PixelDataLength];

                colorFrame.CopyPixelDataTo(colorData);
                if (PaintCanvas.Children.Contains(im) )
                  PaintCanvas.Children.Remove(im);
                if (PaintCanvas.Children.Contains(name))
                    PaintCanvas.Children.Remove(name);
                im = new Image();
                im.Height = SystemParameters.PrimaryScreenHeight;
                im.Width = SystemParameters.PrimaryScreenWidth;
                Canvas.SetTop(im, name.Height);
                im.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                im.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                im.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, colorData, colorFrame.Width * colorFrame.BytesPerPixel);
                //Display only in the standby (inactive) mode
                if (activestate == 0)
                {
                    PaintCanvas.Children.Add(im);
                    PaintCanvas.Children.Add(name);
                }
            }
        }
        
        /*
         This function deals with the skeleton frame data as soon as it is ready
         */        
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
            //copy all the available skeleton frames
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            
            //if the number of skeletons is greater than 0
            if (skeletons.Length != 0)
            {
                //check if any skeleton is being tracked, i.e., it is within the kinect sensor's range,
                //choose this skeleton as the active one
                for (int s = 0; s < skeletons.Length; s++)
                {

                    skel = skeletons[s];

                    if (skel.TrackingState == SkeletonTrackingState.Tracked )
                    {

                        break;
                    }
                }
                //check if this skeleton is being tracked     
                if(skel.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //Switch the system to active mode
                    if (activestate == 0)
                    {
                        //disable color stream and stop displaying video
                        this.sensor.ColorStream.Disable();
                        PaintCanvas.Children.Remove(im);
                        
                        PaintCanvas.Children.Clear();

                        //stop timer
                        timer.Stop();
                        //all nodes should now be in the default state
                        c1.changeClusterState("default");
                        c2.changeClusterState("default");
                        c3.changeClusterState("default");
                        c4.changeClusterState("default");
                        c5.changeClusterState("default");
                        //display the five clusters on screen
                        MainWindow.c1.displayCluster(defNodeProp, 100, 80, 20, false);
                        MainWindow.c2.displayCluster(defNodeProp, 550, 1150, 20, false);
                        MainWindow.c3.displayCluster(defNodeProp, 550, 80, 20, false);
                        MainWindow.c4.displayCluster(defNodeProp, 100, 1150, 20, false);
                        MainWindow.c5.displayCluster(defNodeProp, 440, 650, 20, false);
                        activestate = 1; //system is now in the active state
                        gestureProcessor.RframeCount = 0;//reset left and right hand frame counters
                        gestureProcessor.LframeCount = 0;
                        drawButton(120, 700);//draw the change information layout button


                    }
                    //Change the default system description text from "Get Involved" to "Explore the SIS Department"
                    if (PaintCanvas.Children.Contains(name))
                    {
                        PaintCanvas.Children.Remove(name);
                        name.FontSize = 60;
                        name.Opacity = 0.5;
                        name.Content = "Explore The SIS Department!";
                        name.Visibility = System.Windows.Visibility.Visible;
                        PaintCanvas.Children.Add(name);
                    }
                    else
                    {
                        name.Content = "Explore The SIS Department!";
                        name.FontSize = 60;
                        name.Opacity = 0.5;
                        name.Visibility = System.Windows.Visibility.Visible;
                        PaintCanvas.Children.Add(name);

                    }
                    //Start detecting gestures
                     gp.gestureDetector();

                    }
                    //if the skeleton is not being tracked i.e., out of the sensor's range, switch the system to standby mode
                    else if (skel.TrackingState == SkeletonTrackingState.NotTracked )
                    {
                       //start timer 
                        timer.Start();
                        //system is not active
                        activestate = 0;
                        //reset the gesture processor
                        gp = new gestureProcessor();
                        gestureProcessor.state = "";
                        //remove all visual elements except video and system title
                        for (int i = 0; i < PaintCanvas.Children.Count; i++)
                        {
                            if (PaintCanvas.Children[i] != im && PaintCanvas.Children[i] != name)
                                PaintCanvas.Children.RemoveAt(i);
                        }
                        //enable color stream   
                        this.sensor.ColorStream.Enable();
                        

                    }


            }
        }

        //Right click Event handler (in case mouse is being used). It is not relevant to the gesture interactive prototype
        public static void node_rightClick(object sender, EventArgs e)
        {
            Grid gr = sender as Grid;

            TextBlock txt = gr.Children[1] as TextBlock;
            Label nodeParent = gr.Children[2] as Label;
            Label nodeState = gr.Children[3] as Label;
            Label nodeName = gr.Children[4] as Label;

            if (nodeState.Content == "selected")
            {
                PaintCanvas.Children.Clear();
                c1.changeClusterState("default");
                c2.changeClusterState("default");
                c3.changeClusterState("default");
                c4.changeClusterState("default");
                c5.changeClusterState("default");
                c1.displayCluster(MainWindow.defNodeProp, c1.top, c1.left, 20, false);
                c2.displayCluster(MainWindow.defNodeProp, c2.top, c2.left, 20, false);
                c3.displayCluster(MainWindow.defNodeProp, c3.top, c3.left, 20, false);
                c4.displayCluster(MainWindow.defNodeProp, c4.top, c4.left, 20, false);
                c5.displayCluster(MainWindow.defNodeProp, c5.top, c5.left, 20, false);
                
            }
        }

        //This function deals with left clicks on the node (in case mouse is being used). It is not relevant to the gesture interactive prototype
        public static void node_leftClick(object sender, EventArgs e)
        {
            timer.Stop();
            Grid gr = sender as Grid;
            Ellipse el = gr.Children[0] as Ellipse;
            TextBlock txt = gr.Children[1] as TextBlock;
            Label nodeParent = gr.Children[2] as Label;
            Label nodeState = gr.Children[3] as Label;
            Label nodeName = gr.Children[4] as Label;
            Label nodeType = gr.Children[5] as Label;
            Label nodeSubject = gr.Children[6] as Label;
            if (nodeState.Content == "default" || nodeState.Content == "zoomed" || nodeState.Content == "shrinked")
            {
                if (nodeParent.Content == "c1")
                {

                    c1.selectCluster(zoomedClusProp);
                    c2.changeClusterState("shrinked");
                    c3.changeClusterState("shrinked");
                    c4.changeClusterState("shrinked");
                    c5.changeClusterState("shrinked");
                  /*  c2.displayCluster(shrinkNodeProp,0,0,9 );//80, 100, 9);
                    c3.displayCluster(shrinkNodeProp, 500,950,9 );//750, 1150, 9);
                    c4.displayCluster(shrinkNodeProp, 500,0,9 );//750, 100, 9);
                    c5.displayCluster(shrinkNodeProp,10,950,9 );//60, 1150, 9);
                    */
                    MainWindow.c2.displayCluster(shrinkNodeProp, 40, 20, 9, false);
                    MainWindow.c3.displayCluster(shrinkNodeProp, 600, 1190, 9, false);
                    MainWindow.c4.displayCluster(shrinkNodeProp, 600, 20, 9, false);
                    MainWindow.c5.displayCluster(shrinkNodeProp, 40, 1190, 9, false);
                }
                else if (nodeParent.Content == "c2")
                {
                    c2.selectCluster(zoomedClusProp);
                    c3.changeClusterState("shrinked");
                    c4.changeClusterState("shrinked");
                    c5.changeClusterState("shrinked");
                    c1.changeClusterState("shrinked");
                   /* c1.displayCluster(shrinkNodeProp, 0,0, 9);//80, 100, 9);
                    c3.displayCluster(shrinkNodeProp, 500,950,9 );//750, 1150, 9);
                    c4.displayCluster(shrinkNodeProp, 500,0,9);//750, 100, 9);
                    c5.displayCluster(shrinkNodeProp, 10,950,9);//60, 1150, 9);
                    */
                    MainWindow.c3.displayCluster(shrinkNodeProp, 40, 20, 9, false);
                    MainWindow.c4.displayCluster(shrinkNodeProp, 600, 1190, 9, false);
                    MainWindow.c5.displayCluster(shrinkNodeProp, 600, 20, 9, false);
                    MainWindow.c1.displayCluster(shrinkNodeProp, 40, 1190, 9, false);
                }
                else if (nodeParent.Content == "c3")
                {
                    c3.selectCluster( zoomedClusProp);
                    c1.changeClusterState("shrinked");
                    c2.changeClusterState("shrinked");
                    c4.changeClusterState("shrinked");
                    c5.changeClusterState("shrinked");
                    /*c1.displayCluster(shrinkNodeProp, 0, 0, 9);//80, 100, 9);
                    c2.displayCluster(shrinkNodeProp,500,950,9 );//750, 1150, 9);
                    c4.displayCluster(shrinkNodeProp,500,0,9 );//750, 100, 9);
                    c5.displayCluster(shrinkNodeProp,  10,950,9);//60, 1150, 9);
                    */
                    MainWindow.c1.displayCluster(shrinkNodeProp, 40, 20, 9, false);
                    MainWindow.c2.displayCluster(shrinkNodeProp, 600, 1190, 9, false);
                    MainWindow.c4.displayCluster(shrinkNodeProp, 600, 20, 9, false);
                    MainWindow.c5.displayCluster(shrinkNodeProp, 40, 1190, 9, false);
                }
                else if (nodeParent.Content == "c4")
                {
                    c4.selectCluster(zoomedClusProp);
                    c2.changeClusterState("shrinked");
                    c3.changeClusterState("shrinked");
                    c5.changeClusterState("shrinked");
                    c1.changeClusterState("shrinked");
                  /*  c1.displayCluster(shrinkNodeProp, 0, 0, 9);//80, 100, 9);
                    c2.displayCluster(shrinkNodeProp,500,950,9 );//750, 1150, 9);
                    c3.displayCluster(shrinkNodeProp,500,0,9 );//750, 100, 9);
                    c5.displayCluster(shrinkNodeProp, 10,950,9 );//60, 1150, 9);
                    */
                    MainWindow.c2.displayCluster(shrinkNodeProp, 40, 20, 9, false);
                    MainWindow.c3.displayCluster(shrinkNodeProp, 600, 1190, 9, false);
                    MainWindow.c5.displayCluster(shrinkNodeProp, 600, 20, 9, false);
                    MainWindow.c1.displayCluster(shrinkNodeProp, 20, 1190, 9, false);
                }
                else if (nodeParent.Content == "c5")
                {
                    c5.selectCluster(zoomedClusProp);
                    c2.changeClusterState("shrinked");
                    c3.changeClusterState("shrinked");
                    c4.changeClusterState("shrinked");
                    c1.changeClusterState("shrinked");
                   /* c1.displayCluster(shrinkNodeProp, 0, 0, 9);//80, 100, 9);
                    c2.displayCluster(shrinkNodeProp,500,950,9 );//750, 1150, 9);
                    c3.displayCluster(shrinkNodeProp,500,0,9 );//750, 100, 9);
                    c4.displayCluster(shrinkNodeProp, 10, 950, 9);//60, 1150, 9);
                    */
                    MainWindow.c2.displayCluster(shrinkNodeProp, 40, 20, 9, false);
                    MainWindow.c3.displayCluster(shrinkNodeProp, 600, 1190, 9, false);
                    MainWindow.c4.displayCluster(shrinkNodeProp, 600, 20, 9, false);
                    MainWindow.c1.displayCluster(shrinkNodeProp, 40, 1190, 9, false);
                }
            }
            else if (nodeState.Content == "selected")
            {
                //  MessageBox.Show("node zoomed in");
                if (nodeParent.Content == "c1")
                {


                    for (int i = 0; i < c1.nodes.Count; i++)
                    {

                        if (nodeName.Content == c1.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height!=300)
                        {
                            // MessageBox.Show(c1.nodes.ElementAt(i).content);
                            c1.nodes.ElementAt(i).state = "zoomed";
                            nodeState.Content = "zoomed";
                            txt.Text = c1.nodes.ElementAt(i).content;
                            PaintCanvas.Children.Remove(gr);
                            c1.nodes.ElementAt(i).displayNode(zoomedNodeProp, c1.nodes.ElementAt(i).textColor, Canvas.GetTop(gr), Canvas.GetLeft(gr), PaintCanvas, 1);
                        }


                    }

                }
                else if (nodeParent.Content == "c2")
                {


                    for (int i = 0; i < c2.nodes.Count; i++)
                    {

                        if (nodeName.Content == c2.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                        {
                            // MessageBox.Show(c1.nodes.ElementAt(i).content);
                            c2.nodes.ElementAt(i).state = "zoomed";
                            nodeState.Content = "zoomed";
                            txt.Text = c2.nodes.ElementAt(i).content;
                            PaintCanvas.Children.Remove(gr);
                            c2.nodes.ElementAt(i).displayNode(zoomedNodeProp, c2.nodes.ElementAt(i).textColor, Canvas.GetTop(gr), Canvas.GetLeft(gr), PaintCanvas, 1);
                        }


                    }

                }
                else if (nodeParent.Content == "c3")
                {

                    for (int i = 0; i < c3.nodes.Count; i++)
                    {

                        if (nodeName.Content == c3.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                        {
                            // MessageBox.Show(c1.nodes.ElementAt(i).content);
                            c3.nodes.ElementAt(i).state = "zoomed";
                            nodeState.Content = "zoomed";
                            txt.Text = c3.nodes.ElementAt(i).content;
                            PaintCanvas.Children.Remove(gr);
                            c3.nodes.ElementAt(i).displayNode( zoomedNodeProp, c3.nodes.ElementAt(i).textColor, Canvas.GetTop(gr), Canvas.GetLeft(gr), PaintCanvas,1);
                        }
                        
                       
                    }

                }
                else if (nodeParent.Content == "c4")
                {

                    for (int i = 0; i < c4.nodes.Count; i++)
                    {

                        if (nodeName.Content == c4.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                        {
                            // MessageBox.Show(c1.nodes.ElementAt(i).content);
                            c4.nodes.ElementAt(i).state = "zoomed";
                            nodeState.Content = "zoomed";
                            txt.Text = c4.nodes.ElementAt(i).content;
                            PaintCanvas.Children.Remove(gr);
                            c4.nodes.ElementAt(i).displayNode(zoomedNodeProp, c4.nodes.ElementAt(i).textColor, Canvas.GetTop(gr), Canvas.GetLeft(gr), PaintCanvas,1);
                        }
                        

                    }

                }
                else if (nodeParent.Content == "c5")
                {
                   
                    for (int i = 0; i < c5.nodes.Count; i++)
                    {

                        if (nodeName.Content == c5.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                        {
                            // MessageBox.Show(c1.nodes.ElementAt(i).content);
                            c5.nodes.ElementAt(i).state = "zoomed";
                            nodeState.Content = "zoomed";
                            txt.Text = c5.nodes.ElementAt(i).content;
                            PaintCanvas.Children.Remove(gr);
                            c5.nodes.ElementAt(i).displayNode(zoomedNodeProp, c5.nodes.ElementAt(i).textColor, Canvas.GetTop(gr), Canvas.GetLeft(gr), PaintCanvas,1);
                        }
                        
                      //  short intelligent1 =  new node();



                    }

                }

            }

        }

        //If the window is resized (Not relevant)
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // MessageBox.Show("ok");
            if (this.MinWidth > 0 && this.MinHeight > 0)
            {
                double hscale, wscale;
                hscale = e.NewSize.Height / this.MinHeight;
                wscale = e.NewSize.Width / this.MinWidth;
                PaintCanvas.Height = SystemParameters.PrimaryScreenHeight;
                PaintCanvas.Width = SystemParameters.PrimaryScreenWidth;
                UNCCLearning.LayoutTransform = new ScaleTransform(hscale, wscale);

            }
        }
    }
}
