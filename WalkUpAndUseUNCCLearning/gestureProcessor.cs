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

    public partial class gestureProcessor : Window
    {
           public static float[] righthandxPnts;  //this array stores the right hand's x coordinates in different frames
           public static float[] righthandyPnts;  //this array stores the right hand's y coordinates in different frames
           public static float[] lefthandxPnts;   //this array stores the left hand's x coordinates in different frames
           public static float[] lefthandyPnts;  //this array stores the left hand's y coordinates in different frames
           int highlightCnt;
           public static int RframeCount; //this variable stores the right hand frame count
           public static int LframeCount; //this variable stores the left hand frame count
           public static string state; //this variable represents the state of the node
           public static Ellipse ell = new Ellipse(); //ellipse to represent the right hand cursor
           public static Ellipse ell1 = new Ellipse(); //ellipse to represent the left hand cursor
           public static string currentCluster; //this variable represents the current cluster
           
           //constructor for gestureProcessor object
           public gestureProcessor()
           {
               righthandxPnts = new float[100];
               righthandyPnts = new float[100];
               lefthandxPnts = new float[150];
               lefthandyPnts = new float[150];
               highlightCnt = 0;
               RframeCount = 0;
               LframeCount = 0;
               state = "";
               currentCluster = "";
           }
           
           //destructor for gestureProcessor object
           ~gestureProcessor()
           {
           }

           //This is the actual function which detects gestures, and processes them
           public void gestureDetector()
           {
               //Code to draw the Right hand cursor
               ell.Height = 70;
               ell.Width = 70;
               ell.StrokeThickness = 0;
               ell.Visibility = System.Windows.Visibility.Visible;
               ImageBrush im = new ImageBrush();
               im.ImageSource = new BitmapImage(new Uri(@"pictures/rcursor.jpg", UriKind.Relative));//assign cursor image to this ellipse
               ell.Fill = im;
              
               //To move the right hand cursor along the right hand's position, delete the underlying ellipse at the previous
               //right hand location, and draw a new ellipse at the right hand's new location
               if(MainWindow.PaintCanvas.Children.Contains(ell))
                  MainWindow.PaintCanvas.Children.Remove(ell);
               MainWindow.PaintCanvas.Children.Add(ell);
               Canvas.SetLeft(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);

               //Code to draw Left hand cursor
               ell1.Height = 70;
               ell1.Width = 70;
               ell1.StrokeThickness = 0;
               ell1.Visibility = System.Windows.Visibility.Visible;
               ImageBrush im1 = new ImageBrush();
               im1.ImageSource = new BitmapImage(new Uri(@"pictures/lcursor.jpg", UriKind.Relative));
               ell1.Fill = im1;

               //To move the left hand cursor along the right hand's position, delete the underlying ellipse at the previous
               //left hand location, and draw a new ellipse at the left hand's new location           
               if (MainWindow.PaintCanvas.Children.Contains(ell1))
                   MainWindow.PaintCanvas.Children.Remove(ell1);
               MainWindow.PaintCanvas.Children.Add(ell1);
               Canvas.SetLeft(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);  
              
               //Get the scaled joint locations for right and left hands
               Joint righthand = ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f);
               Joint lefthand = ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f);
               
               /*This code starts checking whether the left/right hand is moved across or pointed at a node or cluster
               If moved across a node/cluster, it is highlighted, if pointed at, the respective node/cluster is selected/displayed
               based on their state*/
               
               //Check which of the visual elements on the screen is pointed at/across
               foreach (FrameworkElement element in MainWindow.PaintCanvas.Children)
               {
                     //if the visual element on the screen is a node grid and
                     //both left and right hands are not pointing at the screen at the same time and
                     //the visual element overlaps with the right hand cursor position or the left hand cursor position
                     //this means the user is trying to point across this node/cluster
                     if (element is Grid && (Math.Abs(righthand.Position.Y - lefthand.Position.Y)>200) && (((Math.Abs(Canvas.GetTop(element) - righthand.Position.Y)) < 110 && (Math.Abs(Canvas.GetLeft(element) - righthand.Position.X)) < 110) || (Math.Abs(Canvas.GetTop(element) - lefthand.Position.Y)) < 110 && (Math.Abs(Canvas.GetLeft(element) - lefthand.Position.X)) < 110))// && lefthand.Position.Z >= elbow.Position.Z)
                     {

                         Grid g = element as Grid;
                    
                         Label nodeState= new Label();
                         Label nodeParent = new Label();
                         Ellipse e = new Ellipse();
                         //get this node's ellipse, its state and its parent cluster id from the grid
                         if (g !=null)
                         {
                             e = g.Children[0] as Ellipse;
                             nodeState = g.Children[3] as Label;
                             nodeParent = g.Children[2] as Label;
                         }
                         //highlight this node's boundary by changing the thickness size, and boundary color to white
                         if (e.Fill != null && e.StrokeThickness==0) 
                         {
                             e.Stroke = new SolidColorBrush(Colors.White);
                             e.StrokeThickness = 6;
                             highlightCnt++;
                             g.Children[0] = e;

                         }
                            
                         
                         // If 100 frames are received for the right hand
                         // Further check if the user was trying to point at the node/cluster to select it.
                         if (RframeCount ==100)
                         {
                             //if maximum number of elements in the right hand x coordinates array are same and
                             //if maximum number of elements in the right hand y coordinates array are same and
                             // the node state is a button
                             //then the user tried to select the "change information layout" button.
                             //Therefore re-cluster the nodes by the other criteria
                             if ((righthandxPnts.Distinct().Count() < 120 && righthandyPnts.Distinct().Count() < 120) && nodeState.Content == "button")// || nodeState.Content == "selected"))
                             {
                                      
                                     RframeCount = 0; //reset right hand frame counter
                                     righthandxPnts = new float[100]; //reset the right hand x and y coordinate array data
                                     righthandyPnts = new float[100];
                                     highlightCnt = 0;
                                     MainWindow.changeClusterReordering(); //call this function to re-cluster nodes
                                     break;

                              }
                            //if maximum number of elements in the right hand x coordinates array are same and
                             //if maximum number of elements in the right hand y coordinates array are same and
                             // the node state is default
                             //then the user tried to select one of the five clusters from the main screen
                             //Therefore enlarge this cluster and shrink others
                              
                              else if ((righthandxPnts.Distinct().Count() < 40 && righthandyPnts.Distinct().Count() < 40) && nodeState.Content == "default")// || nodeState.Content == "selected"))
                              {

                                     RframeCount = 0; //reset right hand frame counter
                                     righthandxPnts = new float[100];//reset the right hand x and y coordinate array data
                                     righthandyPnts = new float[100];
                                     highlightCnt = 0;
                                     state = "default";
                                     processHoverGesture(g); //call this function to process point gesture
                                     break;                                        

                              }
                            //if maximum number of elements in the right hand x coordinates array are same and
                             //if maximum number of elements in the right hand y coordinates array are same and
                             // the node state is selected
                             //then the user tried to select one of the nodes inside the cluster
                             //Therefore enlarge this node and display its contents
                              else if ( (righthandxPnts.Distinct().Count() < 150 && righthandyPnts.Distinct().Count() < 150) && nodeState.Content == "selected")
                              {
                                         
                                         RframeCount = 0; //reset right hand frame counter
                                         righthandxPnts = new float[100];//reset the right hand x and y coordinate array data
                                         righthandyPnts = new float[100];
                                         highlightCnt = 0;
                                         state = "selected";
                                         processHoverGesture(g); //call this function to process point gesture
                                         break;


                               }
                               
                             //if maximum number of elements in the right hand x coordinates array are same and
                             //if maximum number of elements in the right hand y coordinates array are same and
                             // the node state is shrinked
                             //then the user tried to select one of the smaller shrinked clusters
                             //Therefore enlarge this cluster and shrink others                             
                               else if ( (righthandxPnts.Distinct().Count() < 50 && righthandyPnts.Distinct().Count() < 50) && (currentCluster=="" || currentCluster==nodeParent.Content.ToString() ))// || nodeState.Content == "selected"))
                               {
                                         
                                         RframeCount = 0; //reset right hand frame counter
                                         righthandxPnts = new float[100];//reset the right hand x and y coordinate array data
                                         righthandyPnts = new float[100];
                                         highlightCnt = 0;
                                         state = "shrinked";                                      
                                         processHoverOverShrinkedNodes(g);//call this function to process point gesture
                                         break;


                                }
                               //if maximum number of elements in the right hand x coordinates array are same and
                               //if maximum number of elements in the right hand y coordinates array are same and
                               // the node state is zoomed
                               //then the user tried to select one of the zoomed (already selected) nodes in a cluster again
                               //Therefore enlarge this node and display its contents                                 
                                else if ((righthandxPnts.Distinct().Count() < 150 && righthandyPnts.Distinct().Count() < 150) && nodeState.Content == "zoomed")// || nodeState.Content == "selected"))
                                {
                                         RframeCount = 0;//reset right hand frame counter
                                         righthandxPnts = new float[100];//reset the right hand x and y coordinate array data
                                         righthandyPnts = new float[100];
                                         highlightCnt = 0;
                                         state = "zoomed";
                                         processHoverGesture(g);//call this function to process point gesture
                                         break;


                                 }

                                 RframeCount = 0; //reset right hand frame counter even if no point gesture was detected
                           }

                           //if the right and left hand frame counter is not 100, increment them and store the new
                           //right and left hand positions in their respective arrays
                           else
                           {
                                     righthandxPnts[RframeCount] = righthand.Position.X;
                                     righthandyPnts[RframeCount] = righthand.Position.Y;
                                     RframeCount++;
                                     lefthandxPnts[RframeCount] = lefthand.Position.X;
                                     lefthandyPnts[RframeCount] = lefthand.Position.Y;
                                     LframeCount++;
                 
                            }
          
                     }
                     //if the visual element is a node grid, but the hand is not point across or over it
                     //make sure it is not highlighted
                     else if (element is Grid)
                     {
                         Grid g = element as Grid;
                         Ellipse e = g.Children[0] as Ellipse;
                         e.StrokeThickness = 0;
                         g.Children[0] = e;


                     }
                }
                  

           }
           
           //This function deals with scaling a joint point relative to the display screen
           public static Joint ScalePoint(Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
           {
               Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
               {
                   X = Scale1(width, skeletonMaxX, joint.Position.X),
                   Y = Scale1(height, skeletonMaxY, -joint.Position.Y),
                   Z = joint.Position.Z
               };

               joint.Position = pos;

               return joint;
           }


           public static float Scale1(int maxPixel, float maxSkeleton, float position)
           {
               float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
               if (value > maxPixel)
                   return maxPixel;
               if (value < 0)
                   return 0;
               return value;
           }

           public static void processHoverOverShrinkedNodes(Grid gr)
           {
               if (MainWindow.PaintCanvas.Children.Contains(ell))
                   MainWindow.PaintCanvas.Children.Remove(ell);
               MainWindow.PaintCanvas.Children.Add(ell);
               Canvas.SetLeft(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);

               if (MainWindow.PaintCanvas.Children.Contains(ell1))
                   MainWindow.PaintCanvas.Children.Remove(ell1);
               MainWindow.PaintCanvas.Children.Add(ell1);
               Canvas.SetLeft(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);

               Ellipse el = gr.Children[0] as Ellipse;
               TextBlock txt = gr.Children[1] as TextBlock;
               Label nodeParent = gr.Children[2] as Label;
               Label nodeState = gr.Children[3] as Label;
               Label nodeName = gr.Children[4] as Label;
               Label nodeType = gr.Children[5] as Label;
               Label nodeSubject = gr.Children[6] as Label;
               int r = MainWindow.rnd.Next(50, 100);
               if(nodeState.Content == "shrinked" && gestureProcessor.state == "shrinked")
               {
                    gestureProcessor.state = "";
                    gestureProcessor.currentCluster = nodeParent.Content.ToString();
                   if (nodeParent.Content == "c1")
                   {

                       MainWindow.c1.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
         
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);

                   }
                   else if (nodeParent.Content == "c2")
                   {
                       
                       MainWindow.c2.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
              
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   else if (nodeParent.Content == "c3")
                   {
                       MainWindow.c3.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c1.changeClusterState("shrinked");
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
                  
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   else if (nodeParent.Content == "c4")
                   {
                       MainWindow.c4.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
                
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   else if (nodeParent.Content == "c5")
                   {
                       MainWindow.c5.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
                
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
               }
           }


           //Process the point/hover gesture 
           public static void processHoverGesture(Grid gr)
           {
              //This code removes any previous unremoved right hand cursor and adds a new right hand cursor at the current right
              //hand position
               if (MainWindow.PaintCanvas.Children.Contains(ell))
                   MainWindow.PaintCanvas.Children.Remove(ell);
               MainWindow.PaintCanvas.Children.Add(ell);
               Canvas.SetLeft(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell, ScalePoint(MainWindow.skel.Joints[JointType.HandRight], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);
              
              //This code removes any previous unremoved left hand cursor and adds a new left hand cursor at the current left
              //hand position
               if (MainWindow.PaintCanvas.Children.Contains(ell1))
                   MainWindow.PaintCanvas.Children.Remove(ell1);
               MainWindow.PaintCanvas.Children.Add(ell1);
               Canvas.SetLeft(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.X);
               Canvas.SetTop(ell1, ScalePoint(MainWindow.skel.Joints[JointType.HandLeft], (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, 0.3f, 0.3f).Position.Y);

               Ellipse el = gr.Children[0] as Ellipse;
               TextBlock txt = gr.Children[1] as TextBlock;
               Label nodeParent = gr.Children[2] as Label;
               Label nodeState = gr.Children[3] as Label;
               Label nodeName = gr.Children[4] as Label;
               Label nodeType = gr.Children[5] as Label;
               Label nodeSubject = gr.Children[6] as Label;
               int r = MainWindow.rnd.Next(50, 100);
              //check if the node's actual state and the state in the gestureprocessor object is the same as default
              //this is to ensure that this if condition code is satisfied only once. Therefore the gesture processor state is changed to null inside the code  
               if ((nodeState.Content == "default" && gestureProcessor.state == "default") )
               {
                   gestureProcessor.state = ""; //to ensure that the code is exceuted only once
                   gestureProcessor.currentCluster = "";// to ensure that the code is executed only once
                   //if the parent cluster id of the selected node is c1, select this c1 cluster and shrink others
                   if (nodeParent.Content == "c1")
                   {

                       MainWindow.c1.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");

                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);

                   }
                   //if the parent cluster id of the selected node is c2, select this c2 cluster and shrink others
                   else if (nodeParent.Content == "c2")
                   {
                      
                       MainWindow.c2.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
         
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   //if the parent cluster id of the selected node is c3, select this c3 cluster and shrink others
                   else if (nodeParent.Content == "c3")
                   {
                       MainWindow.c3.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c1.changeClusterState("shrinked");
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
        
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   //if the parent cluster id of the selected node is c4, select this c4 cluster and shrink others
                   else if (nodeParent.Content == "c4")
                   {
                       MainWindow.c4.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c5.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
          
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c5.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
                   //if the parent cluster id of the selected node is c5, select this c5 cluster and shrink others
                   else if (nodeParent.Content == "c5")
                   {
                       MainWindow.c5.selectCluster(MainWindow.zoomedClusProp);
                       MainWindow.c2.changeClusterState("shrinked");
                       MainWindow.c3.changeClusterState("shrinked");
                       MainWindow.c4.changeClusterState("shrinked");
                       MainWindow.c1.changeClusterState("shrinked");
         
                       MainWindow.c2.displayCluster(MainWindow.shrinkNodeProp, 90, 20, 13, false);
                       MainWindow.c3.displayCluster(MainWindow.shrinkNodeProp, 550, 1190, 13, false);
                       MainWindow.c4.displayCluster(MainWindow.shrinkNodeProp, 550, 20, 13, false);
                       MainWindow.c1.displayCluster(MainWindow.shrinkNodeProp, 90, 1190, 13, false);
                   }
               }
              //check if the node's actual state and the state in the gestureprocessor object is the same as selected or zoomed
              //this is to ensure that this if condition code is satisfied only once. Therefore the gesture processor state is changed to null inside the code  
               else if ((nodeState.Content == "selected" && gestureProcessor.state == "selected") || (nodeState.Content == "zoomed" && gestureProcessor.state == "zoomed"))// || nodeState.Content == "zoomed")
               {
                   gestureProcessor.state = ""; //to ensure that this if condition code is executed only once
                   gestureProcessor.currentCluster = "";
                   //if the parent cluster id of the selected node is c1
                   if (nodeParent.Content == "c1")
                   {

                       //check which of the nodes inside this cluster has been selected
                       for (int i = 0; i < MainWindow.c1.nodes.Count; i++)
                       {
                           //compare the selected node's name to that of the node i's name, and make sure it is not one of the
                           //title nodes of the cluster
                           if ((nodeName.Content == MainWindow.c1.nodes.ElementAt(i).name) && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                           {
                              
                               MainWindow.c1.nodes.ElementAt(i).state = "zoomed";
                               nodeState.Content = "zoomed";
                               txt.Text = MainWindow.c1.nodes.ElementAt(i).content;
                               MainWindow.PaintCanvas.Children.Remove(gr);
                               MainWindow.c1.nodes.ElementAt(i).displayNode(MainWindow.zoomedNodeProp, MainWindow.c1.nodes.ElementAt(i).textColor, Canvas.GetTop(gr) + 20, Canvas.GetLeft(gr) + 30, MainWindow.PaintCanvas, 1);
                              
                           }


                       }

                   }
                   //if the parent cluster id of the selected node is c2
                   else if (nodeParent.Content == "c2")
                   {
                       //check which of the nodes inside this cluster has been selected      
                       for (int i = 0; i < MainWindow.c2.nodes.Count; i++)
                       {
                            //compare the selected node's name to that of the node i's name, and make sure it is not one of the
                           //title nodes of the cluster                           

                           if ((nodeName.Content == MainWindow.c2.nodes.ElementAt(i).name) && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                           {
                               
                               MainWindow.c2.nodes.ElementAt(i).state = "zoomed";
                               nodeState.Content = "zoomed";
                               txt.Text = MainWindow.c2.nodes.ElementAt(i).content;
                               MainWindow.PaintCanvas.Children.Remove(gr);
                               MainWindow.c2.nodes.ElementAt(i).displayNode(MainWindow.zoomedNodeProp, MainWindow.c2.nodes.ElementAt(i).textColor, Canvas.GetTop(gr) + 20, Canvas.GetLeft(gr) + 30, MainWindow.PaintCanvas, 1);
                               
                           }
                          

                       }

                   }
                   //if the parent cluster id of the selected node is c3
                   else if (nodeParent.Content == "c3")
                   {
                     //check which of the nodes inside this cluster has been selected   
                       for (int i = 0; i < MainWindow.c3.nodes.Count; i++)
                       {
                            //compare the selected node's name to that of the node i's name, and make sure it is not one of the
                           //title nodes of the cluster 
                           if (nodeName.Content == MainWindow.c3.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                           {
                               
                               MainWindow.c3.nodes.ElementAt(i).state = "zoomed";
                               nodeState.Content = "zoomed";
                               txt.Text = MainWindow.c3.nodes.ElementAt(i).content;
                               MainWindow.PaintCanvas.Children.Remove(gr);
                               MainWindow.c3.nodes.ElementAt(i).displayNode(MainWindow.zoomedNodeProp, MainWindow.c3.nodes.ElementAt(i).textColor, Canvas.GetTop(gr) + 20, Canvas.GetLeft(gr) + 30, MainWindow.PaintCanvas, 1);
                               
                           }

                       }

                   }
                   //if the parent cluster id of the selected node is c4
                   else if (nodeParent.Content == "c4")
                   {
                       //check which of the nodes inside this cluster has been selected  
                       for (int i = 0; i < MainWindow.c4.nodes.Count; i++)
                       {
                            //compare the selected node's name to that of the node i's name, and make sure it is not one of the
                           //title nodes of the cluster 
                           if (nodeName.Content == MainWindow.c4.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                           {
                               
                               MainWindow.c4.nodes.ElementAt(i).state = "zoomed";
                               nodeState.Content = "zoomed";
                               txt.Text = MainWindow.c4.nodes.ElementAt(i).content;
                               MainWindow.PaintCanvas.Children.Remove(gr);
                               MainWindow.c4.nodes.ElementAt(i).displayNode(MainWindow.zoomedNodeProp, MainWindow.c4.nodes.ElementAt(i).textColor, Canvas.GetTop(gr) + 20, Canvas.GetLeft(gr) + 30, MainWindow.PaintCanvas, 1);
                               
                           }
                            

                       }

                   }
                   //if the parent cluster id of the selected node is c5
                   else if (nodeParent.Content == "c5")
                   {
                       //check which of the nodes inside this cluster has been selected 
                       for (int i = 0; i < MainWindow.c5.nodes.Count; i++)
                       {
                            //compare the selected node's name to that of the node i's name, and make sure it is not one of the
                           //title nodes of the cluster 
                           if (nodeName.Content == MainWindow.c5.nodes.ElementAt(i).name && (nodeType.Content != "main") && (nodeSubject.Content != "none") && gr.Height != 300)
                           {
                               
                               MainWindow.c5.nodes.ElementAt(i).state = "zoomed";
                               nodeState.Content = "zoomed";
                               txt.Text = MainWindow.c5.nodes.ElementAt(i).content;
                               MainWindow.PaintCanvas.Children.Remove(gr);
                               MainWindow.c5.nodes.ElementAt(i).displayNode(MainWindow.zoomedNodeProp, MainWindow.c5.nodes.ElementAt(i).textColor, Canvas.GetTop(gr) + 20, Canvas.GetLeft(gr) + 30, MainWindow.PaintCanvas, 1);
                               
                           }
                           

                       }

                   }

               }


           }
    
    }


}