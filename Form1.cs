using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace FixedPartitionSimulation
{
    public partial class Form1 : Form
    {
        private int timer = 0;
        private const int kernel = 50;
        private int RAM, processes;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            label4.Text = timer.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Proceed to Table Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (memoryRAMBox.Text == "" || noProcessesBox.Text == "")
                MessageBox.Show("Fields cannot be empty", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                int memoryRAM = 0, noProcesses = 0;
                try
                {
                    memoryRAM = Convert.ToInt32(memoryRAMBox.Text);
                    noProcesses = Convert.ToInt32(noProcessesBox.Text);
                    processes = noProcesses;
                    if (memoryRAM > 5000)
                        MessageBox.Show("RAM Size Exceeded Maximum RAM Requirement!", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (memoryRAM <= 0 || memoryRAM < 1000)
                        MessageBox.Show("RAM Size does not meet the minimum requirement", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (noProcesses > 10)
                        MessageBox.Show("Exceeded Maximum Processes Requirement!", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (noProcesses <= 0 || noProcesses < 5)
                        MessageBox.Show("No. of Processes does not meet the minimum requirement", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        dataGridView1.Show();
                        fillTable(noProcesses);
                        configureMemoryPartitions(memoryRAM, noProcesses);
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("Invalid Input", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        // Allocate the Process ID's in the table
        private void fillTable(int processes)
        {
            for (int i = 0; i < processes; i++)
            {
                string processId = $"P{i + 1}"; // Generate process IDs (P1, P2, ...)
                dataGridView1.Rows.Add(processId); // Add a new row with the process ID
            }
        }

        // List to store added panels from For Loop below
        List<Panel> addedPanels = new List<Panel>();
        private void configureMemoryPartitions(int memoryRAM, int noProcesses)
        {
            memoryRAMPanel.Margin = new Padding(0); // Set zero margin for no spacing
            memoryRAMPanel.Padding = new Padding(0);
            double panelHeight = memoryRAMPanel.Height;
            double panelWidth = memoryRAMPanel.Width;
            double memoryUsable = memoryRAM - 50;

            int[] noPartitionsChoices = { noProcesses - 1, noProcesses, noProcesses - 2 };
            int randomIndex = random.Next(0, 3);
            int newNoProcesses = noPartitionsChoices[randomIndex];
            double heightOfPanels = panelHeight / newNoProcesses;
            decimal partitionSizes = (decimal)memoryUsable / (decimal)newNoProcesses;


            for (int i = 0; i < newNoProcesses; i++)
            {
                Panel panel = new Panel();
                panel.Size = new Size((int)panelWidth, (int)heightOfPanels);
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Margin = new Padding(0);
                Label partitionLabel = new Label();
                partitionSizes = Math.Round(partitionSizes, 2);
                partitionLabel.Text = $"{partitionSizes.ToString()} KB";
                partitionLabel.AutoSize = true;
                panel.Controls.Add(partitionLabel);
                memoryRAMPanel.Controls.Add(panel);
                addedPanels.Add(panel); // every added panel is added to a List, so it can be tracked later
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Hide();
        }
        // Extract Data from Data Grid
        private void button3_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridView1.RowCount;
            int columnCount = dataGridView1.ColumnCount;
            double[,] dataTable = new double[rowCount, columnCount];
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    if (columnIndex == 0)
                        dataTable[rowIndex, columnIndex] = rowIndex + 1;
                    else
                        dataTable[rowIndex, columnIndex] = Convert.ToDouble(row.Cells[columnIndex].Value.ToString());
                }
            }
            checkDataExtracted(dataTable);
            findShortestAllocationTime(dataTable);
        }
        // Find the shortest allocation time Process
        private void findShortestAllocationTime(double[,] dataTable)
        {
            int shortestAllocTimeIndex = 0;
            double currentShortestAllocTime;
            for (int rowIndex = 0; rowIndex < dataTable.GetLength(0) - 1; rowIndex++)
            {

                if (dataTable[(rowIndex), 2] > dataTable[(rowIndex + 1), 2])
                {
                    currentShortestAllocTime = dataTable[(rowIndex + 1), 2];
                    shortestAllocTimeIndex = rowIndex + 1;
                }
            }
            MessageBox.Show("Shortest Allocation time is P" + dataTable[shortestAllocTimeIndex, 0]);
            allocateProcess(dataTable, shortestAllocTimeIndex,1);

        }
        // Allocate Process to a Partition
        private void allocateProcess(double[,] dataTable, int rowIndex, int columnIndex)
        {
            List<double> partitionMemorySizes = new List<double>();
            foreach(Panel panels in addedPanels)
            {
                Label label = panels.Controls.OfType<Label>().FirstOrDefault();

                if (label != null)
                {
                    partitionMemorySizes.Add(Convert.ToDouble(label.Text.ToString()));
                }
            }
            double processMemory = dataTable[rowIndex, columnIndex];
            for(int i = 0; i < partitionMemorySizes.Count; i++)
            {
                if (partitionMemorySizes[i] > processMemory)
                {
                    double panelHeights = addedPanels[i].Height;
                    double percentage = Math.Round((processMemory / panelHeights), 2);
                    addedPanels[i].Invoke(new Action(() =>
                    {
                        double panelWidth = addedPanels[i].Width;
                        double panelHeight = addedPanels[i].Height;

                        // Calculate Height Colored Section
                        double coloredHeight = panelHeight * percentage;

                        // Use Control.CreateGraphics() for drawing
                        using (Graphics g = addedPanels[i].CreateGraphics())
                        {
                            g.FillRectangle(Brushes.Green, 0, 0, (int)panelWidth, (int)coloredHeight);
                        }
                    }));
                    //paintPanel(addedPanels[i], percentage);
                }
            }
        }

        // Paint Process Allocation
       /* private void paintPanel(Panel panel, double percentage)
        {
            panelHeight = added
            // Calculate Height Colored Section
            double coloredHeight = panelHeight * percentage;
            // Draw Color
            addedPanels[i].Invoke(new Action(() =>
            {
                double panelWidth = addedPanels[i].Width;
                double panelHeight = addedPanels[i].Height;

                // Calculate Height Colored Section
                double coloredHeight = panelHeight * percentage;

                // Use Control.CreateGraphics() for drawing
                using (Graphics g = addedPanels[i].CreateGraphics())
                {
                    g.FillRectangle(Brushes.Green, 0, 0, (int)panelWidth, (int)coloredHeight);
                }
            }));

        }*/

        private void newSortedArray(int rowIndex, int columnIndex, double[,] dataTable)
        {
            double[,] newSortedAllocationTimme = new double[dataTable.GetLength(0), dataTable.GetLength(1)];
        }
        // Reset Computer Button to Reconfigure new Partitions for the Memory(RAM)
        private void button2_Click(object sender, EventArgs e)
        {
            memoryRAMPanel.Controls.Clear();
            dataGridView1.Rows.Clear();
            memoryRAMBox.Clear();
            noProcessesBox.Clear();
        }

        private void checkDataExtracted(double[,] dataTable)
        {
            string message = "";

            for (int rowIndex = 0; rowIndex < dataTable.GetLength(0); rowIndex++)
            {
                // Create a string for the current row
                string rowString = "";
                for (int columnIndex = 0; columnIndex < dataTable.GetLength(1); columnIndex++)
                {
                    rowString += dataTable[rowIndex, columnIndex] + " ";
                }

                // Remove the trailing space from the row string
                rowString = rowString.TrimEnd();

                // Add the row string to the message with a newline
                message += rowString + "\n";
            }

            // Remove the trailing newline from the message (optional)
            message = message.TrimEnd('\n');

            // Display the message in a MessageBox
            MessageBox.Show(message, "Data Grid Content");
        }

    }
}

