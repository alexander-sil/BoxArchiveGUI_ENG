using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BoxArchiveGUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private static List<string> filenames = new List<string>();
        public Window1()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = true;
            dialog.Title = "Add Files";

            dialog.InitialDirectory = Directory.GetCurrentDirectory();


            if (dialog.ShowDialog() == true)
            {
                string[] filenames = dialog.FileNames;

                foreach (string i in filenames)
                {
                    Window1.filenames.Add(i);
                    this.FileList.Items.Add(i);
                    this.DeleteSelectedButton.IsEnabled = true;
                }

            }
        }

        private void AcceptAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (filenames.Count > 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();

                saveDialog.AddExtension = true;
                saveDialog.Title = "Save Archive";
                saveDialog.ValidateNames = true;
                saveDialog.Filter = "BOX Files (*.box)|*.BOX";
                saveDialog.OverwritePrompt = false;

                string filename = "C:\\pagefile.sys";

                saveDialog.InitialDirectory = Directory.GetCurrentDirectory();

                if (saveDialog.ShowDialog() == true)
                {
                    filename = saveDialog.FileName;

                    if (new FileInfo(filename).Directory.Name == "output")
                    {
                        MessageBox.Show("Invalid directory name.", "", MessageBoxButton.OK);
                        return;
                    }

                    if (Directory.Exists("canned"))
                    {
                        Directory.Delete("canned", true);
                        Directory.CreateDirectory("canned");

                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }
                    else
                    {
                        Directory.CreateDirectory("canned");

                        DirectoryInfo cannedInfo = new DirectoryInfo("canned");
                        cannedInfo.Attributes |= FileAttributes.Hidden;
                    }

                    if (Directory.Exists("input"))
                    {
                        Directory.Delete("input", true);
                        Directory.CreateDirectory("input");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        inputInfo.Attributes |= FileAttributes.Hidden;
                    }
                    else
                    {
                        Directory.CreateDirectory("input");

                        DirectoryInfo inputInfo = new DirectoryInfo("input");
                        inputInfo.Attributes |= FileAttributes.Hidden;
                    }



                    Logic.PackFilesToInt(filenames.ToArray(), 0);
                    Logic.CanFiles();
                    Logic.BoxCans(filename);
                    MessageBoxResult answer = MessageBox.Show("Show containing directory?", "", MessageBoxButton.YesNo);

                    if (answer == MessageBoxResult.Yes)
                    {
                        Process.Start("C:\\Windows\\explorer.exe", $"\"{new FileInfo(filename).Directory.FullName}\"");
                        this.Close();
                    }
                    else
                    {
                        this.Close();
                    }


                }

                filenames.Clear();
                this.FileList.Items.Clear();
            }
            else
            {
                filenames.Clear();
                this.FileList.Items.Clear();

                MessageBox.Show("No files specified. Aborting compressing procedure.", "", MessageBoxButton.OK);
                this.Close();
            }
        }







        private void AddFilesForm_Closed(object sender, EventArgs e)
        {
            if (Directory.Exists("unboxed")) { Directory.Delete("unboxed", true); }
            if (Directory.Exists("uncanned")) { Directory.Delete("uncanned", true); }

            if (Directory.Exists("input")) { Directory.Delete("input", true); }
            if (Directory.Exists("canned")) { Directory.Delete("canned", true); }
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileList.SelectedIndex != -1)
            {
                MessageBoxResult answer = MessageBox.Show("Confirm deletion?", "", MessageBoxButton.YesNo);

                if (answer == MessageBoxResult.Yes)
                {
                    filenames.Remove(filenames[this.FileList.SelectedIndex]);
                    this.FileList.Items.Remove(this.FileList.Items[this.FileList.SelectedIndex]);
                }

            }

            if ((this.FileList.Items.Count == 0) && (filenames.Count == 0))
            {
                this.DeleteSelectedButton.IsEnabled = false; 
            }
        }

        private void AddFilesForm_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
