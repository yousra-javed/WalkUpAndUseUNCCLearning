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
    //This code deals with reading the database file and loading all the node data to create all the nodes.
    public partial class loadData : Window
    {

        public loadData()
        {

            node n;
            List<string> relNodes = new List<string>();
            StreamReader rd = new StreamReader("d.csv"); //open the csv database of node information

            while (!rd.EndOfStream) //Code to read the node information from the csv file
            {
                
                string[] cells = rd.ReadLine().Split(',');
                //Create a node object
                n = new node(cells.ElementAt(0), cells.ElementAt(1), cells.ElementAt(2), cells.ElementAt(3), cells.ElementAt(4), cells.ElementAt(5), cells.ElementAt(6), cells.ElementAt(7), cells.ElementAt(8));
                MainWindow.Nodes.Add(n);
                relNodes.Add(cells.ElementAt(9)); //add the cell containing semicolon separated node numbers of related nodes for a particular node e.g, 1;2;3
                
            }
            for (int i = 0; i < relNodes.Count; i++) // Code to add related node objects to each node using the semicolon separated numbers as IDs
            {
                if (relNodes.ElementAt(i) != "")
                {
                    
                    var nodeNums = relNodes.ElementAt(i).Split(';'); //related nodes for node i to be added
                    for (int j = 0; j < nodeNums.Count(); j++)
                    {
                        MainWindow.Nodes.ElementAt(i).relNodes.Add(MainWindow.Nodes.ElementAt(Convert.ToInt32(nodeNums[j]) - 1)); // Adding the related nodes
                    }
                }
            }
            




































            /* previously hard coded data (neglect it)
             node n1 = new node("Security", "c1", "main", "Security Information", "", "sec", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Research", "c1", "research", "Research", "", "none", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Events", "c1", "event", "Events", "", "none", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Clubs", "c1", "club", "Clubs", "", "none", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Courses", "c1", "course", "Courses", "", "none", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Professors", "c1", "faculty", "Professors", "", "none", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);

             n1 = new node("49th Security Division", "c1", "club", "Participates in annual competition 'CCDC' Location: Woodward Topics:pen testing, firewalls, social engineering, cryptography", "", "sec", 60, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("User Centric Policy Management for Social Networks", "c1", "research", "Development of a comprehensive and compelling framework that leverages data mining approaches and policies composed by other community members to provide the user with appropriate information required when making policy decisions", "", "sec", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Network and Information Security ", "c1", "course", "ITIS 6167/8167 course", "", "sec", 100, 150, 200, 40, "default", "#ED9A33", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Mohamed Shehab", "c1", "faculty", "Associate Professor in Software and Information Systems. He has an NSF Award, and Google Research Award", "shehab.jpg", "sec", 100, 150, 200, 40, "default", "#2d3f51", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(7));
             MainWindow.Nodes.Add(n1);
             MainWindow.Nodes.ElementAt(7).relNodes.Add(MainWindow.Nodes.ElementAt(9));
             n1 = new node("Carolina Cyber Defender Scholarship", "c1", "event", "All participants will be fully integrated into the department's research programs and become affiliated with an on-going research project", "videos/Security/CyberScholarship.mov", "sec", 100, 150, 200, 40, "default", "#19978A", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Ehab Al-Shaer", "c1", "faculty", "Professor in Computer Science, Director of the Cyber Defense and Network Assurability Center, and NSF IUCRC Center on Security Configuration Analytics and Automation in UNCC", "ehab.png", "sec", 100, 150, 200, 40, "default", "#2d3f51", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(8));
             MainWindow.Nodes.Add(n1);
             MainWindow.Nodes.ElementAt(8).relNodes.Add(MainWindow.Nodes.ElementAt(11));

             n1 = new node("Exploring the Security Capabilities of Physical Layer Network Coding", "c1", "research", "This project seeks to identify capabilities of the PNC technique to detect attacks and design corresponding mechanisms for different network infrastructures and attacker models", "", "sec", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Interactive Static Analysis for Early Detection of Software Vulnerabilities", "c1", "research", "Interactive Static Analysis for Early Detection of Software Vulnerabilities", "", "sec", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Weichao Wang", "c1", "faculty", "Associate Professor in Software and Information Systems", "chao.jpg", "sec", 100, 150, 200, 40, "default", "#2d3f51", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Bill Chu", "c1", "faculty", "Professor in Software and Information Systems", "billchu.jpg", "sec", 100, 150, 200, 40, "default", "#2d3f51", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Intro to Security and Privacy ", "c1", "course", "ITIS 3200 course provides an introductory overview of key issues and solutions for information security and privacy. Prerequisites: ITCS 1213 ", "", "sec", 100, 150, 200, 40, "default", "#ED9A33", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(14));
             MainWindow.Nodes.Add(n1);
             n1 = new node("Secure Programming and Penetration Testing ", "c1", "course", "ITIS 4221 course teaches Techniques for web application penetration testing, secure software development techniques for network based applications. Prerequisites: ITCS 4166 ", "videos/Security/SecureProgramming.mov", "sec", 100, 150, 200, 40, "default", "#ED9A33", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(15));
             MainWindow.Nodes.Add(n1);
             MainWindow.Nodes.ElementAt(12).relNodes.Add(MainWindow.Nodes.ElementAt(14));
             MainWindow.Nodes.ElementAt(14).relNodes.Add(MainWindow.Nodes.ElementAt(12));
             MainWindow.Nodes.ElementAt(14).relNodes.Add(MainWindow.Nodes.ElementAt(16));
             MainWindow.Nodes.ElementAt(13).relNodes.Add(MainWindow.Nodes.ElementAt(15));
             MainWindow.Nodes.ElementAt(15).relNodes.Add(MainWindow.Nodes.ElementAt(13));
             MainWindow.Nodes.ElementAt(15).relNodes.Add(MainWindow.Nodes.ElementAt(17));
             n1 = new node("Human Computer Interaction", "c2", "main", "Human Computer Interaction", "", "hci", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Interact Club", "c2", "club", "Interact club info...", "videos/HCI/Interact.mov", "hci", 60, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Dr. Heather Lipford", "c2", "faculty", "Associate Professor in Software and Information Systems", "heatherlipford.jpg", "hci", 100, 150, 200, 40, "default", "#2d3f51", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Interaction Design Studio", "c2", "course", "ITIS 4011/6011/8011 course is a studio approach to teaching topics in interaction design.", "", "hci", 100, 150, 200, 40, "default", "#ED9A33", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Human Computer Interaction", "c2", "course", "ITIS 3130 course teaches the basics of Human Computer Interaction", "", "hci", 100, 150, 200, 40, "default", "#ED9A33", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(20));
             MainWindow.Nodes.Add(n1);
             n1 = new node("Little Bill", "c2", "research", "Little Bill", "videos/HCI/LittleBill.mov", "hci", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Tangible Creativity", "c2", "research", "Research project using siftables", "videos/HCI/TangibleCreativity.mov", "hci", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Dr. Mary Lou Maher", "c2", "faculty", "Chair of Software and Information Systems", "marylou.jpg", "hci", 100, 150, 200, 40, "default", "#2d3f51", "White");
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(21));
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(24));
             n1.relNodes.Add(MainWindow.Nodes.ElementAt(19));
             MainWindow.Nodes.Add(n1);
             MainWindow.Nodes.ElementAt(21).relNodes.Add(MainWindow.Nodes.ElementAt(25));
             MainWindow.Nodes.ElementAt(20).relNodes.Add(MainWindow.Nodes.ElementAt(22));
             MainWindow.Nodes.ElementAt(24).relNodes.Add(MainWindow.Nodes.ElementAt(25));
             MainWindow.Nodes.ElementAt(19).relNodes.Add(MainWindow.Nodes.ElementAt(25));

             n1 = new node("Software Systems", "c3", "main", "Security Information", "", "sw", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Association for Computing Machinery", "c3", "club", "Association for Computing Machinery", "", "sw", 60, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Research", "c3", "research", "Coming Soon", "", "sw", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("IT Infrastructure", "c3", "course", "IT Infrastructure", "/videos/SoftSys/ITIS2110.mov", "sw", 100, 150, 200, 40, "default", "#ED9A33", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("ACM Meeting", "c3", "event", "ACM Meeting", "", "sw", 100, 150, 200, 40, "default", "#19978A", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Thomas Kitrick", "c3", "faculty", "Thomas Kitrick", "", "sw", 100, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);

             n1 = new node("Health Informatics", "c4", "main", "Health Informatics Information", "", "hcip", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Lixia Yao", "c4", "faculty", "Lixia Yao", "", "hcip", 60, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Yaorong Ge", "c4", "research", "Yaorong Ge", "", "hcip", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Masters Program", "c4", "course", "Masters Program", "videos/HealthInfo/HealthInformatics.mov", "hcip", 100, 150, 200, 40, "default", "#ED9A33", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Information Session", "c4", "event", "Information Session ", "videos/HealthInfo/HIInfoSession.mov", "hcip", 100, 150, 200, 40, "default", "#19978A", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Workshop in Next-Generation Sequence Analysis and Metabolomics", "c4", "event", "Workshop in Next-Generation Sequence Analysis and Metabolomics", "", "hcip", 100, 150, 200, 40, "default", "#19978A", "White");
             MainWindow.Nodes.Add(n1);


             n1 = new node("Intelligence and Information Systems", "c5", "main", "Intelligence and Information Systems Information", "", "iis", 150, 250, 300, 60, "default", "Yellow", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("David Wilson", "c5", "faculty", "David Wilson", "", "iis", 60, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Artificial Intelligence", "c5", "course", "Coming Soon", "videos/InfoSys/ArtificialIntelligence.mov", "iis", 100, 150, 200, 40, "default", "#C13045", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Topics in Software & Info Syst: Network Science", "c5", "course", "Topics in Software & Info Syst: Network Science", "", "iis", 100, 150, 200, 40, "default", "#ED9A33", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Introduction to High-Performance Computing: Techniques and Applications", "c5", "event", "Introduction to High-Performance Computing: Techniques and Applications", "", "iis", 100, 150, 200, 40, "default", "#19978A", "White");
             MainWindow.Nodes.Add(n1);
             n1 = new node("Mirsad Hadzikadic", "c5", "faculty", "Mirsad Hadzikadic", "", "iis", 100, 150, 200, 40, "default", "#6C4C77", "White");
             MainWindow.Nodes.Add(n1);
             */
            
        }



     }
}