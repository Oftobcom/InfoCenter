using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace KML_TA
{
    class Program
    {
        public static string sConnectionString = "";
        public static double MR_Threshold = 0.1;

        static void Main(string[] args)
        {
            // Operating System Environment Culture
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            string sAttr;
            string Data_Source = "";
            string User_ID = "";
            string Password = "";
            string Initial_Catalog = "";
            string Directory_Path = "";
            string General_File_Name = "";

            try
            {
                // Initialize runtime settings
                // Read Data Source number from the config file            
                Data_Source = ConfigurationManager.AppSettings.Get("Data Source");
                Console.WriteLine("Data Source: " + Data_Source);

                // Read User ID from the config file            
                User_ID = ConfigurationManager.AppSettings.Get("User ID");
                Console.WriteLine("User ID: " + User_ID);

                // Read User Password from the config file            
                Password = ConfigurationManager.AppSettings.Get("Password");
                Console.WriteLine("Password: " + Password);

                // Initial Catalog, Default Database
                Initial_Catalog = ConfigurationManager.AppSettings.Get("Initial_Catalog");
                Console.WriteLine("Initial Catalog: " + Initial_Catalog);

                // Connection String
                sConnectionString = "Provider=SQLOLEDB.1;User ID=" + User_ID + ";Password=" + Password +
                    "; Data Source=" + Data_Source + "; Initial Catalog=" + Initial_Catalog + 
                    ";Persist Security Info=True;";
                Console.WriteLine("DB Connection String: " + sConnectionString);

                // Read MRs Fraction threshold from the config file
                // MR_Num_Threshold is used to filter the TAs for which arcs are plotted
                sAttr = ConfigurationManager.AppSettings.Get("MRs Fraction threshold percent");
                MR_Threshold = Double.Parse(sAttr) / 100.0;
                Console.WriteLine("MRs Fraction threshold: " + MR_Threshold);

                Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");
                General_File_Name = ConfigurationManager.AppSettings.Get("General Information File name");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }


            KML_2G_Hua("S");
            KML_2G_Hua("N");
            KML_3G_Hua("S");
            KML_3G_Hua("N");
            KML_2G_Zte("S");
            KML_2G_Zte("N");
            KML_3G_Zte("S");
            KML_3G_Zte("N");
            KML_4G_Hua("S");
            KML_4G_Hua("N");

            KML_2G_Hua_Max("S");
            KML_2G_Hua_Max("N");
            KML_3G_Hua_Max("S");
            KML_3G_Hua_Max("N");
            KML_2G_Zte_Max("S");
            KML_2G_Zte_Max("N");
            KML_3G_Zte_Max("S");
            KML_3G_Zte_Max("N");

            Converter conv = new Converter();
            conv.ReadCell(2);
            conv.convert1(Directory_Path + "\\" + General_File_Name, 2);
            conv.ReadCell(3);
            conv.convert1(Directory_Path + "\\" + General_File_Name, 3);
            conv.ReadCell(4);
            conv.convert1(Directory_Path + "\\" + General_File_Name, 4);

            KML_2G_RLF_Hua("S");
            KML_2G_RLF_Hua("N");

            Console.WriteLine("Done");
            //Console.ReadLine();
        }

        static void KML_2G_Hua(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            int counters_num = 0;
            int TA_granul = 0;
            int MR_start = 0;
            double distance;
            double azimuth;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("2G TA granularity");
                // Convert string to int
                TA_granul = Int32.Parse(sAttr);
                Console.WriteLine("2G TA granularity: " + TA_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 2G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 2G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("2G TA MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 2G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei Select Query 2G");
                    Console.WriteLine("Huawei Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 2G File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei Select Query 2G");
                    Console.WriteLine("Huawei Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 2G File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_2G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_900.kml");
                kml.Left_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_1800.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_900.kml");
                kml.Right_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_1800.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_900.kml");
                kml.Main_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_1800.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                kml.Arc_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_900.kml");
                kml.Arc_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_1800.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();            
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                //cell.band = Convert.ToDouble(myReader[_band]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                    //Console.WriteLine("MRs : {0} : {1}", i, myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
                //Console.ReadLine();
            }

            myConnection.Close();

            double m_per_d_long;
            double m_per_d_lat;
            double arc_step = TA_granul * MR_Threshold;
            int MR_step = (int)(100 * MR_Threshold);
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.band + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                if(c.band == 900)
                {
                    kml_arc_header = kml.Arc_Header_900;
                    kml_Left_Line_Header = kml.Left_Line_Header_900;
                    kml_Main_Line_Header = kml.Main_Line_Header_900;
                    kml_Right_Line_Header = kml.Right_Line_Header_900;
                }
                else
                {
                    kml_arc_header = kml.Arc_Header_1800;
                    kml_Left_Line_Header = kml.Left_Line_Header_1800;
                    kml_Main_Line_Header = kml.Main_Line_Header_1800;
                    kml_Right_Line_Header = kml.Right_Line_Header_1800;
                }

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TA " + Hua_2G_TA_Range(t.TA_number) + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = Hua_2G_TA_Distance(t.TA_number) * TA_granul;
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = Hua_2G_TA_Distance(c.TA_Max) * TA_granul;
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>2G TA Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 2G Done");
            //Console.ReadLine();
        }

        static void KML_3G_Hua(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            double distance;
            double azimuth;
            int counters_num = 0;
            int MR_start = 0;
            int TP_granul = 0;
            double arc_step = 0;
            int MR_step = 0;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("3G TP granularity");
                // Convert string to int
                TP_granul = Int32.Parse(sAttr);
                Console.WriteLine("3G TP granularity: " + TP_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 3G TP Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 3G TP Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("3G TP MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 3G TP MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei Select Query 3G");
                    Console.WriteLine("Huawei Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 3G File name");
                }
                else 
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei Select Query 3G");
                    Console.WriteLine("Huawei Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 3G File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_3G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_3G.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_3G.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_3G.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                //kml.Arc_Header = System.IO.File.ReadAllText(@"Arc_Header.kml");
                kml.Arc_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_3G.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.ReadLine();

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            arc_step = TP_granul * MR_Threshold;
            //Console.WriteLine("arc_step: " + arc_step);
            MR_step = (int)(100 * MR_Threshold);
            //Console.WriteLine("MR_step: " + MR_step);
            //Console.ReadLine();

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                    //Console.WriteLine("MRs : {0} : {1}", i, myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
               // Console.ReadLine();
            }

            double m_per_d_long;
            double m_per_d_lat;
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                kml_arc_header = kml.Arc_Header_3G;
                kml_Left_Line_Header = kml.Left_Line_Header_3G;
                kml_Main_Line_Header = kml.Main_Line_Header_3G;
                kml_Right_Line_Header = kml.Right_Line_Header_3G;

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TP" + Hua_3G_TP_Range[t.TA_number] + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = Hua_3G_TP_Distance[t.TA_number];
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = Hua_3G_TP_Distance[c.TA_Max];
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>3G TP Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 3G Done");
            //Console.ReadLine();
        }

        static void KML_3G_Zte(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            double distance;
            double azimuth;
            int counters_num = 0;
            int MR_start = 0;
            int TP_granul = 0;
            double arc_step = 0;
            int MR_step = 0;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("3G TP granularity");
                // Convert string to int
                TP_granul = Int32.Parse(sAttr);
                Console.WriteLine("3G TP granularity: " + TP_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("ZTE 3G TP Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("ZTE 3G TP Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("3G TP MRs 1st field, ZTE");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 3G TP MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North ZTE Select Query 3G");
                    Console.WriteLine("ZTE Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North ZTE 3G File name");
                }
                else 
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South ZTE Select Query 3G");
                    Console.WriteLine("ZTE Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South ZTE 3G File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_3G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_3G.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_3G.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_3G.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                //kml.Arc_Header = System.IO.File.ReadAllText(@"Arc_Header.kml");
                kml.Arc_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_3G.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.ReadLine();

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            arc_step = TP_granul * MR_Threshold;
            //Console.WriteLine("arc_step: " + arc_step);
            MR_step = (int)(100 * MR_Threshold);
            //Console.WriteLine("MR_step: " + MR_step);
            //Console.ReadLine();

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                    //Console.WriteLine("MRs : {0} : {1}", i, myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
                //Console.ReadLine();
            }

            double m_per_d_long;
            double m_per_d_lat;
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                kml_arc_header = kml.Arc_Header_3G;
                kml_Left_Line_Header = kml.Left_Line_Header_3G;
                kml_Main_Line_Header = kml.Main_Line_Header_3G;
                kml_Right_Line_Header = kml.Right_Line_Header_3G;

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TP" + ZTE_3G_TP_Range[t.TA_number] + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = ZTE_3G_TP_Distance[t.TA_number];
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = ZTE_3G_TP_Distance[c.TA_Max];
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>3G TP Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("ZTE 3G Done");
            //Console.ReadLine();
        }

        static void KML_2G_Zte(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            int counters_num = 0;
            int TA_granul = 0;
            int MR_start = 0;
            double distance;
            double azimuth;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("2G TA granularity");
                // Convert string to int
                TA_granul = Int32.Parse(sAttr);
                Console.WriteLine("2G TA granularity: " + TA_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("ZTE 2G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("ZTE 2G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("2G TA MRs 1st field, ZTE");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 2G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North ZTE Select Query 2G");
                    Console.WriteLine("ZTE Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North ZTE 2G File name");
                }
                else 
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South ZTE Select Query 2G");
                    Console.WriteLine("ZTE Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South ZTE 2G File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_2G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_900.kml");
                kml.Left_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_1800.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_900.kml");
                kml.Right_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_1800.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_900.kml");
                kml.Main_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_1800.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                kml.Arc_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_900.kml");
                kml.Arc_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_1800.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                    //Console.WriteLine("MRs : {0} : {1}", i, myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
                //Console.ReadLine();
            }

            myConnection.Close();

            double m_per_d_long;
            double m_per_d_lat;
            double arc_step = TA_granul * MR_Threshold;
            int MR_step = (int)(100 * MR_Threshold);
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                if (c.band == 900)
                {
                    kml_arc_header = kml.Arc_Header_900;
                    kml_Left_Line_Header = kml.Left_Line_Header_900;
                    kml_Main_Line_Header = kml.Main_Line_Header_900;
                    kml_Right_Line_Header = kml.Right_Line_Header_900;
                }
                else
                {
                    kml_arc_header = kml.Arc_Header_1800;
                    kml_Left_Line_Header = kml.Left_Line_Header_1800;
                    kml_Main_Line_Header = kml.Main_Line_Header_1800;
                    kml_Right_Line_Header = kml.Right_Line_Header_1800;
                }

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TA " + Zte_2G_TA_Range(t.TA_number) + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = Zte_2G_TA_Distance(t.TA_number) * TA_granul;
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = Zte_2G_TA_Distance(c.TA_Max) * TA_granul;
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>2G TA Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd") + ")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("ZTE 2G Done");
            //Console.ReadLine();
        }

        static void KML_4G_Hua(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            double distance;
            double azimuth;
            int counters_num = 0;
            int MR_start = 0;
            int TP_granul = 0;
            double arc_step = 0;
            int MR_step = 0;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("4G TA granularity");
                // Convert string to int
                TP_granul = Int32.Parse(sAttr);
                Console.WriteLine("4G TA granularity: " + TP_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 4G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 4G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("4G TA MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 4G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei Select Query 4G");
                    Console.WriteLine("Huawei Select Query 4G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 4G File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei Select Query 4G");
                    Console.WriteLine("Huawei Select Query 4G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 4G File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_4G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_4G = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_4G.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_4G = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_4G.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_4G = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_4G.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                //kml.Arc_Header = System.IO.File.ReadAllText(@"Arc_Header.kml");
                kml.Arc_Header_4G = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_4G.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.ReadLine();

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _district = myReader.GetOrdinal("DISTRICT");

            arc_step = TP_granul * MR_Threshold;
            //Console.WriteLine("arc_step: " + arc_step);
            MR_step = (int)(100 * MR_Threshold);
            //Console.WriteLine("MR_step: " + MR_step);
            //Console.ReadLine();

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            double m_per_d_long;
            double m_per_d_lat;
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                kml_arc_header = kml.Arc_Header_4G;
                kml_Left_Line_Header = kml.Left_Line_Header_4G;
                kml_Main_Line_Header = kml.Main_Line_Header_4G;
                kml_Right_Line_Header = kml.Right_Line_Header_4G;

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TP" + Hua_4G_TP_Range[t.TA_number] + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = Hua_4G_TP_Distance[t.TA_number];
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = Hua_4G_TP_Distance[c.TA_Max];
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>4G TA Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 4G Done");
            //Console.ReadLine();
        }

        static void KML_2G_Hua_Max(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            int counters_num = 0;
            int TA_granul = 0;
            int MR_start = 0;
            double distance;
            double azimuth;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("2G TA granularity");
                // Convert string to int
                TA_granul = Int32.Parse(sAttr);
                Console.WriteLine("2G TA granularity: " + TA_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 2G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 2G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("2G TA MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 2G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei Select Query 2G");
                    Console.WriteLine("Huawei Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 2G TA Max File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei Select Query 2G");
                    Console.WriteLine("Huawei Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 2G TA Max File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_2G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_900.kml");
                kml.Left_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_1800.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_900.kml");
                kml.Right_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_1800.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_900.kml");
                kml.Main_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_1800.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                kml.Arc_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_900.kml");
                kml.Arc_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_1800.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= 0.01)
                    {
                        //TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = i - MR_start;
                        // Portion of MRs per TA of cell
                        cell.TA_Max_percent = (uint)Math.Round(MR_fraction * 100);
                        //cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            myConnection.Close();

            double m_per_d_long;
            double m_per_d_lat;
            double arc_step = TA_granul * MR_Threshold;
            int MR_step = (int)(100 * MR_Threshold);
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                if (c.band == 900)
                {
                    kml_arc_header = kml.Arc_Header_900;
                    kml_Left_Line_Header = kml.Left_Line_Header_900;
                    kml_Main_Line_Header = kml.Main_Line_Header_900;
                    kml_Right_Line_Header = kml.Right_Line_Header_900;
                }
                else
                {
                    kml_arc_header = kml.Arc_Header_1800;
                    kml_Left_Line_Header = kml.Left_Line_Header_1800;
                    kml_Main_Line_Header = kml.Main_Line_Header_1800;
                    kml_Right_Line_Header = kml.Right_Line_Header_1800;
                }

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                kml_ta += "<Folder><name>TP" + Hua_2G_TA_Range(c.TA_Max) + " (" + c.TA_Max_percent + "%)</name>\n";
                distance = Hua_2G_TA_Distance(c.TA_Max) * TA_granul;
                for (int i = 0; i < c.TA_Max_percent; i += MR_step)
                {
                    kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                    distance -= arc_step;
                }
                kml_ta += "</Folder>\n";

                kml_temp.Append(kml_ta);

                distance = Hua_2G_TA_Distance(c.TA_Max) * TA_granul;
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>2G TA Maximum (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 2G Max Done");
            //Console.ReadLine();
        }

        static void KML_3G_Hua_Max(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            double distance;
            double azimuth;
            int counters_num = 0;
            int MR_start = 0;
            int TP_granul = 0;
            double arc_step = 0;
            int MR_step = 0;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("3G TP granularity");
                // Convert string to int
                TP_granul = Int32.Parse(sAttr);
                Console.WriteLine("3G TP granularity: " + TP_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 3G TP Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 3G TP Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("3G TP MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 3G TP MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei Select Query 3G");
                    Console.WriteLine("Huawei Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 3G TP Max File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei Select Query 3G");
                    Console.WriteLine("Huawei Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 3G TP Max File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_3G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_3G.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_3G.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_3G.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                //kml.Arc_Header = System.IO.File.ReadAllText(@"Arc_Header.kml");
                kml.Arc_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_3G.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.ReadLine();

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            arc_step = TP_granul * MR_Threshold;
            //Console.WriteLine("arc_step: " + arc_step);
            MR_step = (int)(100 * MR_Threshold);
            //Console.WriteLine("MR_step: " + MR_step);
            //Console.ReadLine();

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= 0.01)
                    {
                        //TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = i - MR_start;
                        // Portion of MRs per TA of cell
                        cell.TA_Max_percent = (uint)Math.Round(MR_fraction * 100);
                        //cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            double m_per_d_long;
            double m_per_d_lat;
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                kml_arc_header = kml.Arc_Header_3G;
                kml_Left_Line_Header = kml.Left_Line_Header_3G;
                kml_Main_Line_Header = kml.Main_Line_Header_3G;
                kml_Right_Line_Header = kml.Right_Line_Header_3G;

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                kml_ta += "<Folder><name>TP" + Hua_3G_TP_Range[c.TA_Max] + " (" + c.TA_Max_percent + "%)</name>\n";
                distance = Hua_3G_TP_Distance[c.TA_Max];
                for (int i = 0; i < c.TA_Max_percent; i += MR_step)
                {
                    kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                    distance -= arc_step;
                }
                kml_ta += "</Folder>\n";

                kml_temp.Append(kml_ta);

                distance = Hua_3G_TP_Distance[c.TA_Max];
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>3G TP Maximum (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 3G Max Done");
            //Console.ReadLine();
        }

        static void KML_2G_Zte_Max(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            int counters_num = 0;
            int TA_granul = 0;
            int MR_start = 0;
            double distance;
            double azimuth;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("2G TA granularity");
                // Convert string to int
                TA_granul = Int32.Parse(sAttr);
                Console.WriteLine("2G TA granularity: " + TA_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("ZTE 2G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("ZTE 2G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("2G TA MRs 1st field, ZTE");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 2G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North ZTE Select Query 2G");
                    Console.WriteLine("ZTE Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North ZTE 2G TA Max File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South ZTE Select Query 2G");
                    Console.WriteLine("ZTE Select Query 2G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South ZTE 2G TA Max File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_2G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_900.kml");
                kml.Left_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_1800.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_900.kml");
                kml.Right_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_1800.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_900.kml");
                kml.Main_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_1800.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                kml.Arc_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_900.kml");
                kml.Arc_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_1800.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= 0.01)
                    {
                        //TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = i - MR_start;
                        // Portion of MRs per TA of cell
                        cell.TA_Max_percent = (uint)Math.Round(MR_fraction * 100);
                        //cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            myConnection.Close();

            double m_per_d_long;
            double m_per_d_lat;
            double arc_step = TA_granul * MR_Threshold;
            int MR_step = (int)(100 * MR_Threshold);
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                if (c.band == 900)
                {
                    kml_arc_header = kml.Arc_Header_900;
                    kml_Left_Line_Header = kml.Left_Line_Header_900;
                    kml_Main_Line_Header = kml.Main_Line_Header_900;
                    kml_Right_Line_Header = kml.Right_Line_Header_900;
                }
                else
                {
                    kml_arc_header = kml.Arc_Header_1800;
                    kml_Left_Line_Header = kml.Left_Line_Header_1800;
                    kml_Main_Line_Header = kml.Main_Line_Header_1800;
                    kml_Right_Line_Header = kml.Right_Line_Header_1800;
                }

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                kml_ta += "<Folder><name>TP" + Zte_2G_TA_Range(c.TA_Max) + " (" + c.TA_Max_percent + "%)</name>\n";
                distance = Zte_2G_TA_Distance(c.TA_Max) * TA_granul;
                for (int i = 0; i < c.TA_Max_percent; i += MR_step)
                {
                    kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                    distance -= arc_step;
                }
                kml_ta += "</Folder>\n";

                kml_temp.Append(kml_ta);

                distance = Zte_2G_TA_Distance(c.TA_Max) * TA_granul;
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>2G TA Maximum (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("ZTE 2G Max Done");
            //Console.ReadLine();
        }

        static void KML_3G_Zte_Max(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            double distance;
            double azimuth;
            int counters_num = 0;
            int MR_start = 0;
            int TP_granul = 0;
            double arc_step = 0;
            int MR_step = 0;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("3G TP granularity");
                // Convert string to int
                TP_granul = Int32.Parse(sAttr);
                Console.WriteLine("3G TP granularity: " + TP_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("ZTE 3G TP Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("ZTE 3G TP Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("3G TP MRs 1st field, ZTE");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 3G TP MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North ZTE Select Query 3G");
                    Console.WriteLine("ZTE Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North ZTE 3G TP Max File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South ZTE Select Query 3G");
                    Console.WriteLine("ZTE Select Query 3G: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South ZTE 3G TP Max File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_3G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_3G.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_3G.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_3G.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                //kml.Arc_Header = System.IO.File.ReadAllText(@"Arc_Header.kml");
                kml.Arc_Header_3G = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_3G.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.ReadLine();

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            arc_step = TP_granul * MR_Threshold;
            //Console.WriteLine("arc_step: " + arc_step);
            MR_step = (int)(100 * MR_Threshold);
            //Console.WriteLine("MR_step: " + MR_step);
            //Console.ReadLine();

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= 0.01)
                    {
                        //TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = i - MR_start;
                        // Portion of MRs per TA of cell
                        cell.TA_Max_percent = (uint)Math.Round(MR_fraction * 100);
                        //cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            double m_per_d_long;
            double m_per_d_lat;
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                kml_arc_header = kml.Arc_Header_3G;
                kml_Left_Line_Header = kml.Left_Line_Header_3G;
                kml_Main_Line_Header = kml.Main_Line_Header_3G;
                kml_Right_Line_Header = kml.Right_Line_Header_3G;

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                kml_ta += "<Folder><name>TP" + ZTE_3G_TP_Range[c.TA_Max] + " (" + c.TA_Max_percent + "%)</name>\n";
                distance = ZTE_3G_TP_Distance[c.TA_Max];
                for (int i = 0; i < c.TA_Max_percent; i += MR_step)
                {
                    kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                    distance -= arc_step;
                }
                kml_ta += "</Folder>\n";

                kml_temp.Append(kml_ta);

                distance = Hua_3G_TP_Distance[c.TA_Max];
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>3G TP Maximum (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("ZTE 3G Max Done");
            //Console.ReadLine();
        }

        static void KML_2G_RLF_Hua(string part)
        {
            List<Cell> Cells = new List<Cell>();
            string sAttr;
            int counters_num = 0;
            int TA_granul = 0;
            int MR_start = 0;
            double distance;
            double azimuth;
            KML_Content kml = new KML_Content();
            KML_Point point_A = new KML_Point();
            KML_Point point_B = new KML_Point();
            //string coordinates;
            string mySelectQuery = "";
            //string kml_content;
            StringBuilder kml_temp = new StringBuilder();
            string kml_ta;
            //string kml_arc;

            try
            {
                // Initialize runtime settings
                // TA - GSM cellular mobile phone standard Timing Advance
                // MR - Measurement Report
                // Read TA granularity from the config file         
                sAttr = ConfigurationManager.AppSettings.Get("2G TA granularity");
                // Convert string to int
                TA_granul = Int32.Parse(sAttr);
                Console.WriteLine("2G TA granularity: " + TA_granul);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("Huawei 2G TA Counters number");
                // Convert string to int
                counters_num = Int32.Parse(sAttr);
                Console.WriteLine("Huawei 2G TA Counters number: " + counters_num);

                // Read TA Counters number from the config file            
                sAttr = ConfigurationManager.AppSettings.Get("2G TA MRs 1st field, Huawei");
                MR_start = Int32.Parse(sAttr);
                Console.WriteLine("Number of 1st field of 2G TA MRs: " + MR_start);

                kml.Directory_Path = ConfigurationManager.AppSettings.Get("Directory path");

                if (part == "N")
                {
                    // Read Select Query from the config file            
                    mySelectQuery = ConfigurationManager.AppSettings.Get("North Huawei 2G RLF Select Query");
                    Console.WriteLine("Huawei 2G RLF Select Query: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("North Huawei 2G RLF File name");
                }
                else
                {
                    mySelectQuery = ConfigurationManager.AppSettings.Get("South Huawei 2G RLF Select Query");
                    Console.WriteLine("Huawei 2G RLF Select Query: " + mySelectQuery);
                    kml.File_Name = ConfigurationManager.AppSettings.Get("South Huawei 2G RLF File name");
                }

                string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Header_2G.kml");
                kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
                kml.Left_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_900.kml");
                kml.Left_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Header_1800.kml");
                kml.Left_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Left_Line_Footer.kml");
                kml.Right_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_900.kml");
                kml.Right_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Header_1800.kml");
                kml.Right_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Right_Line_Footer.kml");
                kml.Main_Line_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_900.kml");
                kml.Main_Line_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Header_1800.kml");
                kml.Main_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Main_Line_Footer.kml");
                kml.Arc_Header_900 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_900.kml");
                kml.Arc_Header_1800 = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Header_1800.kml");
                kml.Arc_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Arc_Footer.kml");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            // Connect to Oracle Database (DB)
            OleDbConnection myConnection = new OleDbConnection(sConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbDataReader myReader = null;

            myConnection.Open();
            try
            {
                myReader = myCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Get fields IDs
            int _cell = myReader.GetOrdinal("CELL_NAME");
            int _latitude = myReader.GetOrdinal("LATITUDE");
            int _longitude = myReader.GetOrdinal("LONGITUDE");
            int _azimuth = myReader.GetOrdinal("AZIMUTH");
            int _beam_h = myReader.GetOrdinal("BEAM_H");
            int _band = myReader.GetOrdinal("BAND");
            int _district = myReader.GetOrdinal("DISTRICT");

            while (myReader.Read()) // Read Site and Cell data from DB
            {
                double MR_total = 0; // total MRs for cell
                double MR_fraction;
                Cell cell = new Cell();
                //Get cell's data
                cell.cell_name = (string)myReader[_cell];
                cell.longitude = Convert.ToDouble(myReader[_longitude]);
                cell.latitude = Convert.ToDouble(myReader[_latitude]);
                cell.azimuth = Convert.ToDouble(myReader[_azimuth]);
                cell.beam_h = Convert.ToDouble(myReader[_beam_h]);
                cell.band = Convert.ToDouble(myReader[_band]);
                cell.district = (string)myReader[_district];

                //Console.WriteLine(myReader[_cell] + "; " + myReader[_longitude] + "; " + myReader[_latitude] + "; " + myReader[_azimuth] + "; ");

                // Calculate total number of MRs per cell
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    MR_total += Convert.ToDouble(myReader[i]);
                }
                //Console.WriteLine("Total number of MRs per {0}: {1}", myReader[_cell], MR_total);
                //Console.WriteLine("Total number of MRs fields: {0}", i);

                //int maxValue = anArray.Max();
                //int maxIndex = anArray.ToList().IndexOf(maxValue);
                for (int i = MR_start; i < counters_num + MR_start; i++)
                {
                    // Calculate MRs fraction per TA
                    MR_fraction = Convert.ToDouble(myReader[i]) / MR_total;
                    // Filter the TAs for which arcs are plotted
                    if (MR_fraction >= MR_Threshold)
                    {
                        TA ta = new TA();
                        // Number of TA of cell
                        cell.TA_Max = (ta.TA_number = i - MR_start);
                        // Portion of MRs per TA of cell
                        ta.TA_MR_percent = (uint)Math.Round(MR_fraction * 100);
                        cell.TAs.Add(ta);
                    }
                }
                // Add Cell information to Cells List
                Cells.Add(cell);
            }

            myConnection.Close();

            double m_per_d_long;
            double m_per_d_lat;
            double arc_step = TA_granul * MR_Threshold;
            int MR_step = (int)(100 * MR_Threshold);
            string kml_arc_header;
            string kml_Left_Line_Header;
            string kml_Main_Line_Header;
            string kml_Right_Line_Header;
            string region1 = "";
            string region2 = "";
            kml_temp.Append(kml.Header);
            //var list = list.OrderByDescending(x => x.Product.Name).ThenBy(x => x.Product.Price).ToList();
            foreach (Cell c in Cells)
            {
                region2 = c.district;
                if (region1 != region2)
                {
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    if (region1 != "")
                    {
                        kml_temp.Append("</Folder>\n");
                    }

                    kml_temp.Append("<Folder><name>");
                    kml_temp.Append(region2);
                    kml_temp.Append("</name>\n");
                    region1 = region2;
                    //Console.WriteLine("region:" + region1 + "; district: " + region2);
                    //Console.ReadLine();
                }

                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.longitude + "; " + c.latitude + "; " + c.azimuth + "; " + c.beam_h + "; " + c.TA_Max + "; ");
                //Console.WriteLine(c.district + "; " + c.cell_name + "; " + c.band + "; ");
                m_per_d_long = WGS.MetersPerDegreeLong(c.latitude);
                m_per_d_lat = WGS.MetersPerDegreeLat(c.latitude);
                // Open folder for cell
                kml_temp.Append("<Folder><name>");
                kml_temp.Append(c.cell_name);
                kml_temp.Append("</name>\n");
                point_A = (KML_Point)c;

                if (c.band == 900)
                {
                    kml_arc_header = kml.Arc_Header_900;
                    kml_Left_Line_Header = kml.Left_Line_Header_900;
                    kml_Main_Line_Header = kml.Main_Line_Header_900;
                    kml_Right_Line_Header = kml.Right_Line_Header_900;
                }
                else
                {
                    kml_arc_header = kml.Arc_Header_1800;
                    kml_Left_Line_Header = kml.Left_Line_Header_1800;
                    kml_Main_Line_Header = kml.Main_Line_Header_1800;
                    kml_Right_Line_Header = kml.Right_Line_Header_1800;
                }

                //kml_ta = "<Folder><name>TAs</name>\n";
                kml_ta = "";
                foreach (TA t in c.TAs)
                {
                    //coordinates = "";
                    //Console.WriteLine("TA number: " + t.TA_number + "; " + "TA MR percent: " + t.TA_MR_percent + "; ");
                    kml_ta += "<Folder><name>TA " + Hua_2G_TA_Range(t.TA_number) + " (" + t.TA_MR_percent + "%)</name>\n";
                    distance = Hua_2G_TA_Distance(t.TA_number) * TA_granul;
                    for (int i = MR_step; i <= t.TA_MR_percent; i += MR_step)
                    {
                        kml_ta += kml_arc_header + ArcCoords(c, distance, m_per_d_long, m_per_d_lat) + kml.Arc_Footer;
                        distance -= arc_step;
                    }
                    kml_ta += "</Folder>\n";
                }
                //kml_ta += "</Folder>\n";
                kml_temp.Append(kml_ta);

                distance = Hua_2G_TA_Distance(c.TA_Max) * TA_granul;
                kml_temp.Append(kml_Left_Line_Header);
                azimuth = c.azimuth - c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Left_Line_Footer);

                kml_temp.Append(kml_Main_Line_Header);
                azimuth = c.azimuth;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Main_Line_Footer);

                kml_temp.Append(kml_Right_Line_Header);
                azimuth = c.azimuth + c.beam_h / 2.0;
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                kml_temp.Append("\t\t");
                kml_temp.Append((string)point_A);
                kml_temp.Append(" ");
                kml_temp.Append((string)point_B);
                kml_temp.Append("\n");
                kml_temp.Append(kml.Right_Line_Footer);

                // Close folder for cell
                kml_temp.Append("</Folder>\n");

            }
            if (region1 != "")
            {
                kml_temp.Append("</Folder>\n");
            }
            kml_temp.Append("<name>2G RLF on TA Distribution (");
            kml_temp.Append(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            kml_temp.Append(")</name>");
            kml_temp.Append(kml.Footer);

            string kml_file = kml.Directory_Path + kml.File_Name;
            try
            {
                using (StreamWriter wr = new StreamWriter(kml_file))
                {
                    wr.Write(kml_temp);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            Console.WriteLine("Huawei 2G Done");
            //Console.ReadLine();
        }

        static string ArcCoords(Cell c, double distance, double m_per_d_long, double m_per_d_lat)
        {
            double azimuth;
            KML_Point point_A = (KML_Point)c;
            KML_Point point_B;
            string coordinates = "";

            int i_end = 5;
            int i_left = -i_end/2;
            int i_right = i_end/2;
            double azimuth_step = c.beam_h / (i_end - 1);
            azimuth = c.azimuth + azimuth_step * i_left;
            for (double i = i_left; i <= i_right; i++)
            {
                point_B = WGS.CalcCoords(point_A, azimuth, distance, m_per_d_long, m_per_d_lat);
                coordinates += (string)point_B + " ";
                azimuth += azimuth_step;
            }

            return "\t\t" + coordinates + "\n";
        }

        static double Hua_2G_TA_Distance(int TA_number)
        {
            if (TA_number < 30)
                return TA_number + 1;
            else if (TA_number == 30)
                return 32;
            else if (TA_number == 31)
                return 34;
            else if (TA_number == 32)
                return 36;
            else if (TA_number == 33)
                return 38;
            else if (TA_number == 34)
                return 40;
            else if (TA_number == 35)
                return 45;
            else if (TA_number == 36)
                return 50;
            else if (TA_number == 37)
                return 55;
            else if (TA_number == 38)
                return 64;
            else
                return 65;
        }

        static string Hua_2G_TA_Range(int TA_number)
        {
            if (TA_number < 30)
                return TA_number.ToString();
            else if (TA_number == 30)
                return "30_31";
            else if (TA_number == 31)
                return "32_33";
            else if (TA_number == 32)
                return "34_35";
            else if (TA_number == 33)
                return "36_37";
            else if (TA_number == 34)
                return "38_39";
            else if (TA_number == 35)
                return "40_44";
            else if (TA_number == 36)
                return "45_49";
            else if (TA_number == 37)
                return "50_54";
            else if (TA_number == 38)
                return "55_63";
            else
                return "63_MORE";
        }

        static double Zte_2G_TA_Distance(int TA_number)
        {
            if (TA_number <= 30)
                return TA_number * 2 + 1;
            else if (TA_number == 31)
                return 64;
            else
                return 65;
        }

        static string Zte_2G_TA_Range(int TA_number)
        {
            if (TA_number == 0)
                return "0";
            else if (TA_number <= 30)
                return (TA_number * 2-1).ToString() + "_" + (TA_number * 2).ToString();
            else if (TA_number == 31)
                return "61_63";
            else
                return "64_MORE";
        }

        public static readonly double[] Hua_3G_TP_Distance = { 234, 468, 702, 936, 1170, 1404, 2340, 3744, 6084, 8424, 13104, 15000 };
        public static readonly string[] Hua_3G_TP_Range = { "_0", "_1", "_2", "_3", "_4", "_5", "_6_9", "_10_15", "_16_25", "_26_35", "_36_55", "_56_MORE" };

        public static readonly double[] Hua_4G_TP_Distance = { 156, 312, 624, 1092, 2028, 3588, 6630, 14508, 30108, 53508, 76908, 90000 };
        public static readonly string[] Hua_4G_TP_Range = { "_0_1", "_2_3", "_4_7", "_8_13", "_14_25", "_26_45", "_46_84", "_86_185", "_186_385", "_386_685", "_686_985", "_985_MORE" };

        public static readonly double[] ZTE_3G_TP_Distance = { 234, 703, 1172, 1641, 2109, 2578, 3281, 3984, 4688, 5391, 6328, 7266, 
            8203, 9141, 10078, 11953, 13828, 15703, 17578, 19453, 21328, 25078, 28828, 32578, 36328, 40078, 47578, 55078, 62578, 
            70078, 77578, 85078, 100078, 115078, 130078, 160078, 190078, 220078, 240000 };
        public static readonly string[] ZTE_3G_TP_Range = { "_234", "_703", "_1172", "_1641", "_2109", "_2578", "_3281", "_3984", 
            "_4688", "_5391", "_6328", "_7266", "_8203", "_9141", "_10078", "_11953", "_13828", "_15703", "_17578", "_19453", 
            "_21328", "_25078", "_28828", "_32578", "_36328", "_40078", "_47578", "_55078", "_62578", "_70078", "_77578", "_85078", 
            "_100078", "_115078", "_130078", "_160078", "_190078", "_220078", "_240000" };

    }

    class Cell
    {
        public List<TA> TAs = new List<TA>(); // List for TA numbers and their MRs' fractions
        public string district;
        public string cell_name;
        public double longitude;
        public double latitude;
        public double altitude = 0;
        public double azimuth;
        public double beam_h; // horizontal beamwidth
        public double band; // Bandwidth: 900, 1800
        public int TA_Max; // greatest TA of cell
        public uint TA_Max_percent = 0;

        static public implicit operator KML_Point(Cell c)
        {
            KML_Point p = new KML_Point();
            p.longitude = c.longitude;
            p.latitude = c.latitude;
            p.altitude = c.altitude;
            return p;
        }
    }

    class TA
    {
        public int TA_number;
        public uint TA_MR_percent;
    }
}

class Cell2
{
    public string cell;
    public string bts_name;
    public double longitude;
    public double latitude;
    public double azimuth;
    public double mech_tilt;
    public double electr_tilt;

    static public implicit operator KML_Point(Cell2 c)
    {
        KML_Point p = new KML_Point();
        p.longitude = c.longitude;
        p.latitude = c.latitude;
        p.altitude = 0;
        return p;
    }
}

class Folder
{
    public static string begin(string name)
    {//todo:нужно вынести на верх папки
        return string.Format("\n<Folder><name>{0}</name>\n<visibility>0</visibility><open>0</open><Style><ListStyle><listItemType>check</listItemType>" +
        "<ItemIcon><state>open</state><href>{1}</href></ItemIcon>" +
        "<ItemIcon><state>closed</state><href>{1}</href></ItemIcon><bgColor>00ffffff</bgColor></ListStyle></Style>", name, Constants.IconEifell);
    }
    public static string end()
    {
        return "\n</Folder>";
    }
}

static class Constants
{
    public static string IconRepeater = @"png\R.png";

    public static string IconCircleGrey = @"png\1.png";
    public static string IconCircleIndigo = @"png\2.png";

    public static string IconInfoN = @"png\icon62.png";
    public static string IconInfoH = @"png\icon54.png";

    public static string IconEifell = @"png\eiffel100.png";

    public static string[] IconNumber = {   @"png\N1.png", @"png\N2.png", @"png\N3.png", 
                                                @"png\N4.png", @"png\N5.png", @"png\N6.png",
                                                @"png\N7.png", @"png\N8.png", @"png\N9.png",
                                                @"png\N10.png", @"png\N11.png", @"png\N12.png",
                                                @"png\N13.png", @"png\N14.png", @"png\N15.png",
                                                @"png\N16.png", @"png\N17.png", @"png\N18.png", 
                                                @"png\R1.png"                                
                                            };

}

class Icon
{
    public Icon() { }
    public Icon(double lon, double lat, string nam, string description, string style)
    {
        coor.longitude = lon;
        coor.latitude = lat;

        this.name = nam;
        this.description = description;

        this.styleId = style;
    }

    public string name;
    public string description;

    public Coordinate coor = new Coordinate();

    public double range = 100000;
    public double tilt = 2.276046722119997e-012;
    public double heading = -8.309716950277599;

    public string styleId;

    public override string ToString()
    {
        string str = string.Format("<Placemark><description>{8}</description><name>{0}</name><LookAt><longitude>{1}</longitude><latitude>{2}</latitude><altitude>0</altitude><range>{3}</range>" +
        "<tilt>{4}</tilt><heading>{5}</heading></LookAt><styleUrl>#msn_{6}</styleUrl><Point><coordinates>{1},{2},0</coordinates></Point></Placemark>", name,
        coor.longitude.ToString().Replace(",", "."), coor.latitude.ToString().Replace(",", "."), range.ToString().Replace(",", "."), tilt.ToString().Replace(",", "."), heading.ToString().Replace(",", "."), styleId, description);

        return str;
    }
    public string ToStringShort()
    {
        return string.Format("<Placemark><description>{0}</description><name>{1}</name><styleUrl>#msn_{2}</styleUrl><Point><extrude>0</extrude><altitudeMode>relativeToGround</altitudeMode><coordinates>{3}</coordinates></Point></Placemark>",
            description, name, styleId, coor);
    }
}

class IconStyle
{
    public static string Style(string styleId, uint color, string path_a, string path_b)
    {
        return string.Format("<StyleMap id=\"msn_{0}\"><Pair><key>normal</key><styleUrl>#sn_{0}</styleUrl></Pair><Pair><key>highlight</key><styleUrl>#sh_{0}</styleUrl></Pair></StyleMap>" +
            "<Style id=\"sn_{0}\"><IconStyle><color>{1}</color><scale>0.5</scale><Icon><href>{2}</href></Icon></IconStyle></Style>" +
            "<Style id=\"sh_{0}\"><IconStyle><color>{1}</color><scale>0.9</scale><Icon><href>{3}</href></Icon></IconStyle></Style>", styleId, (color == 0xffffffff ? "" : color.ToString("x8")), path_a, path_b);
    }

    public static string Style(string styleId, uint color, string path_a, string path_b, string scale_a, string scale_b, string scale_lb_a, string scale_lb_b)
    {
        return string.Format("<StyleMap id=\"msn_{0}\"><Pair><key>normal</key><styleUrl>#sn_{0}</styleUrl></Pair><Pair><key>highlight</key><styleUrl>#sh_{0}</styleUrl></Pair></StyleMap>" +
            "<Style id=\"sn_{0}\"><IconStyle><color>{1}</color><scale>{4}</scale><Icon><href>{2}</href></Icon></IconStyle><LabelStyle><scale>{6}</scale></LabelStyle></Style>" +
            "<Style id=\"sh_{0}\"><IconStyle><color>{1}</color><scale>{5}</scale><Icon><href>{3}</href></Icon></IconStyle><LabelStyle><scale>{7}</scale></LabelStyle></Style>", styleId, (color == 0xffffffff ? "" : color.ToString("x8")), path_a, path_b, scale_a, scale_b, scale_lb_a, scale_lb_b);
    }
}

class Coordinate
{
    public double longitude;
    public double latitude;

    public override string ToString()
    {
        return longitude.ToString().Replace(",", ".") + "," + latitude.ToString().Replace(",", ".") + " ";
    }
}

public class KML_Point
{
    public double longitude;
    public double latitude;
    public double altitude;

    static public implicit operator string(KML_Point p)
    {
        return p.longitude + "," + p.latitude + "," + p.altitude;
    }
}

class KML_Content
{
    public string Directory_Path;
    public string File_Name;
    public string Header;
    public string Footer;
    public string Left_Line_Header_900;
    public string Left_Line_Header_1800;
    public string Left_Line_Header_3G;
    public string Left_Line_Header_4G;
    public string Left_Line_Footer;
    public string Right_Line_Header_900;
    public string Right_Line_Header_1800;
    public string Right_Line_Header_3G;
    public string Right_Line_Header_4G;
    public string Right_Line_Footer;
    public string Main_Line_Header_900;
    public string Main_Line_Header_1800;
    public string Main_Line_Header_3G;
    public string Main_Line_Header_4G;
    public string Main_Line_Footer;
    public string Arc_Header_900;
    public string Arc_Header_1800;
    public string Arc_Header_3G;
    public string Arc_Header_4G;
    public string Arc_Footer;
}

class KML_Azimuth
{
    public string Header;
    public string Footer;
    public string Azimuth_Line_Header;
    public string Azimuth_Line_Footer;
}

public static class WGS
{
    public static double MetersPerDegreeLat(double latitude)
    {
        double lat_r = latitude * Math.PI / 180.0; // Latitude in radians
        double m_per_d = 111132.92 - 559.82 * Math.Cos(2 * lat_r) + 1.175 * Math.Cos(4 * lat_r);
        return m_per_d;
    }

    public static double MetersPerDegreeLong(double latitude)
    {
        double lat_r = latitude * Math.PI / 180.0; // Latitude in radians
        double m_per_d = 111412.84 * Math.Cos(lat_r) - 93.5 * Math.Cos(3 * lat_r) + 0.118 * Math.Cos(5 * lat_r);
        return m_per_d;
    }

    // p_start - coordinates of point in the Earth: longitude, latitude, azimuth
    // azimuth - azimuth of the move direction
    // distance - distance from start point to end point
    public static KML_Point CalcCoords(KML_Point p_start, double azimuth, double distance, double m_per_d_long, double m_per_d_lat)
    {
        //Console.WriteLine("p_start.longitude: " + p_start.longitude);
        //Console.WriteLine("p_start.latitude: " + p_start.latitude);
        //Console.WriteLine("azimuth: " + azimuth);
        //Console.WriteLine("distance: " + distance);
        double delt_m_long; // Longitude delta (meter)
        double delt_m_lat; // Latitude delta (meter)
        KML_Point p_end = new KML_Point();
        double angle = Math.PI * azimuth / 180.0;
        p_end.altitude = p_start.altitude;
        delt_m_long = distance * Math.Sin(angle);
        delt_m_lat = distance * Math.Cos(angle);
        //Console.WriteLine("delt_m_long: " + delt_m_long); // Longitude delta (meter)
        //Console.WriteLine("delt_m_lat: " + delt_m_lat); // Longitude delta (meter)
        p_end.longitude = p_start.longitude + delt_m_long / m_per_d_long;
        p_end.latitude = p_start.latitude + delt_m_lat / m_per_d_lat;

        return p_end;
    }
}

class Converter
{
    public List<Cell2> Cells = new List<Cell2>();

    public void ReadCell(int g)
    {
        // g - Network Generation
        string Data_Source = "";
        string ConnectionString = "";
        string User_ID = "";
        string Password = "";
        string Initial_Catalog = "";
        string mySelectQuery = "";

        try
        {
            // Initialize runtime settings
            // Read Data Source number from the config file            
            Data_Source = ConfigurationManager.AppSettings.Get("Data Source");
            Console.WriteLine("Data Source: " + Data_Source);

            // Read User ID from the config file            
            User_ID = ConfigurationManager.AppSettings.Get("User ID");
            Console.WriteLine("User ID: " + User_ID);

            // Read User Password from the config file            
            Password = ConfigurationManager.AppSettings.Get("Password");
            Console.WriteLine("Password: " + Password);

            // Initial Catalog, Default Database
            Initial_Catalog = ConfigurationManager.AppSettings.Get("Initial_Catalog");
            Console.WriteLine("Initial Catalog: " + Initial_Catalog);

            // Connection String
            ConnectionString = "Provider=SQLOLEDB.1;User ID=" + User_ID + ";Password=" + Password +
                "; Data Source=" + Data_Source + "; Initial Catalog=" + Initial_Catalog +
                ";Persist Security Info=True;";
            Console.WriteLine("DB Connection String: " + ConnectionString);

            // Read Select Query from the config file
            mySelectQuery = ConfigurationManager.AppSettings.Get("General Information Select Query " + g +"G");
            Console.WriteLine("General Information Select Query: " + mySelectQuery);
        }
        catch (FormatException e)
        {
            Console.WriteLine(e.Message);
        }

        // Connect to Oracle Database (DB)
        OleDbConnection myConnection = new OleDbConnection(ConnectionString);
        OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
        OleDbDataReader reader = null;

        myConnection.Open();
        try
        {
            reader = myCommand.ExecuteReader();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            int _bts = reader.GetOrdinal("BTS_NAME");
            int _cell = reader.GetOrdinal("cell");
            int _latitude = reader.GetOrdinal("LATITUDE");
            int _longitude = reader.GetOrdinal("LONGITUDE");
            int _azimuth = reader.GetOrdinal("AZIMUTH");
            int _mech_tilt = reader.GetOrdinal("MECH_TILT");
            int _electr_tilt = reader.GetOrdinal("ELECTR_TILT");

            //Console.WriteLine(_bts + ", " + _cell + ", " + _latitude + ", " + _longitude + ", " + _azimuth);

            Cells.Clear();

            //show parametr nls

            while (reader.Read())
            {
                Cell2 c = new Cell2();

                c.cell = (reader[_cell] == DBNull.Value ? null : reader[_cell].ToString());
                c.bts_name = (reader[_bts] == DBNull.Value ? null : reader[_bts].ToString());

                bool b1 = double.TryParse(reader[_longitude].ToString(), out c.longitude);
                bool b2 = double.TryParse(reader[_latitude].ToString(), out c.latitude);
                bool b3 = double.TryParse(reader[_azimuth].ToString(), out c.azimuth);
                bool b4 = double.TryParse(reader[_mech_tilt].ToString(), out c.mech_tilt);
                bool b5 = double.TryParse(reader[_electr_tilt].ToString(), out c.electr_tilt);

                if (b1 && b2)
                    Cells.Add(c);
            }

            double a = 0.0003;
            var cc = Cells.First();
            string lastBtsName = cc.bts_name;
            double lastLongitude = cc.longitude;
            double lastLatitude = cc.latitude;

            foreach (var x in Cells)
            {
                if (x.bts_name != lastBtsName)
                {
                    lastBtsName = x.bts_name;
                }
                if (Math.Abs(x.longitude - lastLongitude) > a && Math.Abs(x.latitude - lastLatitude) > a)
                {
                    lastLongitude = x.longitude;
                    lastLatitude = x.latitude;
                }
            }
            //------------------------------------------------------
            reader.Close();
        }
        catch (Exception ex)
        {
            Console.Write(ex);
        }
    }

    public void convert1(string path, int g)
    {
        path += "_" + g + "G.kml";
        string begin = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><kml xmlns=\"http://earth.google.com/kml/2.1\"><Document><name>General " + g + "G.kml</name>";
        string end = "</Document></kml>";
        string style = "";
        //string kml_content;
        string bts_descript = "";
        StringBuilder temp = new StringBuilder();
        KML_Azimuth kml = new KML_Azimuth();
        KML_Point point_A = new KML_Point();
        KML_Point point_B = new KML_Point();
        double distance = 1000;
        double m_per_d_long;
        double m_per_d_lat;

        try
        {
            string program_directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            kml.Header = System.IO.File.ReadAllText(program_directory + "\\" + "Azimuth.kml");
            kml.Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Footer.kml");
            kml.Azimuth_Line_Header = System.IO.File.ReadAllText(program_directory + "\\" + "Azimuth_Line_Header.kml");
            kml.Azimuth_Line_Footer = System.IO.File.ReadAllText(program_directory + "\\" + "Azimuth_Line_Footer.kml");
        }
        catch (FormatException e)
        {
            Console.WriteLine(e.Message);
        }

        style += IconStyle.Style("circle", 0xffffffff, Constants.IconCircleIndigo, Constants.IconCircleIndigo);

        for (int i = 0; i < Cells.Count; i++)
        {
            if (i == 0 || Cells[i - 1].bts_name != Cells[i].bts_name)
            {
                temp.Append(Folder.begin(Cells[i].bts_name));
            }

            m_per_d_long = WGS.MetersPerDegreeLong(Cells[i].latitude);
            m_per_d_lat = WGS.MetersPerDegreeLat(Cells[i].latitude);
            point_A = (KML_Point)Cells[i];
            point_B = WGS.CalcCoords(point_A, Cells[i].azimuth, distance, m_per_d_long, m_per_d_lat);
            temp.Append(kml.Azimuth_Line_Header);
            temp.Append("\t\t");
            temp.Append((string)point_A);
            temp.Append(" ");
            temp.Append((string)point_B);
            temp.Append("\n");
            temp.Append(kml.Azimuth_Line_Footer);
            //temp.Append(kml_content);

            //bts_descript += Cells[i].cell + " - " + Cells[i].azimuth + "<br/>";
            bts_descript += Cells[i].cell + " | azim=" + Cells[i].azimuth + " | m.tilt=" + Cells[i].mech_tilt + " | e.tilt=" + Cells[i].electr_tilt + "<br/>";

            if (i == Cells.Count - 1 || Cells[i + 1].bts_name != Cells[i].bts_name)
            {
                //Console.WriteLine("bts description: " + bts_descript);
                Icon ic = new Icon(Cells[i].longitude, Cells[i].latitude, Cells[i].bts_name, bts_descript, "circle");
                temp.Append(ic.ToStringShort());
                temp.Append(Folder.end());
                bts_descript = "";
            }
        }

        string res = begin + style + temp + end;
        try
        {
            using (StreamWriter wr = new StreamWriter(path))
            {
                wr.Write(res);
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex);
        }
    }

}

//KML colors work like so,
//<color>AABBGGRR</color>
//AA = alpha opacity
//BB = blue
//GG = gren
//RR = red