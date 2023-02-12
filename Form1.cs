namespace Sims2ResolutionChanger
{
    public partial class Form1 : Form
    {
        private string[] installedModsDirs = Array.Empty<string>();

        private readonly ILogger logger;
        private readonly string pathToGraphicRulesFile;
        private const string PARAMNAME1 = "option ScreenModeResolution";

        public Form1()
        {
            InitializeComponent();

            pathToGraphicRulesFile = $"{Path.DirectorySeparatorChar}TSData{Path.DirectorySeparatorChar}Res{Path.DirectorySeparatorChar}Config{Path.DirectorySeparatorChar}Graphics Rules.sgr";
            logger = new Logger(textBox1);

            SetTargetResolutionFromScreen();
        }

        private void SetTargetResolutionFromScreen()
        {
            Screen[] screens = Screen.AllScreens;

            int maxScreenWidth = 800;
            int maxScreenHeight = 600;

            foreach (Screen screen in screens)
            {
                if (screen != null)
                {
                    logger.Log($"Found screen with resolution {screen.Bounds.Width}x{screen.Bounds.Height}");

                    if (maxScreenWidth < screen.Bounds.Width && maxScreenHeight < screen.Bounds.Height) 
                    {
                        maxScreenWidth = screen.Bounds.Width;
                        maxScreenHeight = screen.Bounds.Height;
                    } 
                }
            }

            numericUpDown1.Value = maxScreenWidth;
            numericUpDown2.Value = maxScreenHeight;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = folderBrowserDialog1.ShowDialog();

            label2.Text = folderBrowserDialog1.SelectedPath;

            installedModsDirs = Directory.GetDirectories(folderBrowserDialog1.SelectedPath, "*", SearchOption.TopDirectoryOnly);

            foreach (var installedModsDir in installedModsDirs)
            {
                listBox1.Items.Add(Path.GetFileName(installedModsDir));
            }

            if (ValidateFiles())
            {
                button10.Enabled = true;
            }
            else
            {
                MessageBox.Show($"Something went wrong during validation!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ValidateFiles()
        {
            foreach (var installedModsDir in installedModsDirs) 
            {
                string filepath = installedModsDir + pathToGraphicRulesFile;

                if (File.Exists(filepath))
                {                    
                    logger.Log($"Open file from folder \"{Path.GetFileName(installedModsDir)}\"");

                    string[] fileLines = File.ReadAllLines(filepath);

                    int entryLineIndex = Array.FindIndex(fileLines, line => line.Contains(PARAMNAME1));

                    string maxResWidthLine = fileLines[entryLineIndex + 2];
                    string maxResHeightLine = fileLines[entryLineIndex + 3];


                    if (int.TryParse(new string(maxResWidthLine.Where(char.IsDigit).ToArray()), out int currentWidth))
                    {
                        if (!Enumerable.Range(800, 10000).Contains(currentWidth))
                        {
                            logger.Log($"Width value {currentWidth} is out of normal range! Please check {filepath} file before performing any actions.");
                        }
                    } 
                    else
                    {
                        return false;
                    }

                    if (int.TryParse(new string(maxResHeightLine.Where(char.IsDigit).ToArray()), out int currentHeight))
                    {
                        if (!Enumerable.Range(600, 10000).Contains(currentHeight))
                        {
                            logger.Log($"Height value {currentHeight} is out of normal range! Please check {filepath} file before performing any actions.");
                        }
                    }
                    else
                    {
                        return false;
                    }

                    logger.Log($"Verified resolution {currentWidth}x{currentHeight}");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = WriteNewResolution();

                if (result)
                {
                    MessageBox.Show($"Target resolution now changed to {numericUpDown1.Value}x{numericUpDown2.Value}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Something went wrong during writing new values!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool WriteNewResolution() 
        {
            if (!ValidateFiles()) return false;

            logger.Log($"Applying new resolution {numericUpDown1.Value}x{numericUpDown2.Value}");

            foreach (var installedModsDir in installedModsDirs)
            {
                string filepath = installedModsDir + pathToGraphicRulesFile;

                if (File.Exists(filepath))
                {
                    logger.Log($"Working on {filepath}");

                    string[] fileLines = File.ReadAllLines(filepath);

                    int entryLineIndex = Array.FindIndex(fileLines, line => line.Contains(PARAMNAME1));
                    int maxResWidthLineIndex = entryLineIndex + 2;
                    int maxResHeightLineIndex = entryLineIndex + 3;

                    string maxResWidthLine = fileLines[maxResWidthLineIndex];
                    string maxResHeightLine = fileLines[maxResHeightLineIndex];


                    if (int.TryParse(new string(maxResWidthLine.Where(char.IsDigit).ToArray()), out int currentWidth))
                    {
                        maxResWidthLine = maxResWidthLine.Replace(currentWidth.ToString(), numericUpDown1.Value.ToString());
                    }
                    else
                    {
                        return false;
                    }

                    if (int.TryParse(new string(maxResHeightLine.Where(char.IsDigit).ToArray()), out int currentHeight))
                    {
                        maxResHeightLine = maxResHeightLine.Replace(currentHeight.ToString(), numericUpDown2.Value.ToString());
                    }
                    else
                    {
                        return false;
                    }

                    fileLines[maxResWidthLineIndex] = maxResWidthLine;
                    fileLines[maxResHeightLineIndex] = maxResHeightLine;

                    logger.Log($"Current value is {currentWidth}x{currentHeight}, new value is {numericUpDown1.Value}x{numericUpDown2.Value}");

                    File.WriteAllLines(filepath, fileLines);
                }
                else
                {
                    return false;
                }
            }


            return true; 
        }

        private void resolutionPresetButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            string widthValue = button.Text[..button.Text.IndexOf('x')];
            string heightValue = button.Text.Substring(button.Text.IndexOf('x') + 1, button.Text.Length - button.Text.IndexOf('x') - 1);

            numericUpDown1.Value = Convert.ToDecimal(widthValue);
            numericUpDown2.Value = Convert.ToDecimal(heightValue);
        }
    }
}