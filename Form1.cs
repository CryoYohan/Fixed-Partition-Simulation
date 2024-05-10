using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
                int memoryRAM = Convert.ToInt32(memoryRAMBox.Text);
                int noProcesses = Convert.ToInt32(noProcessesBox.Text);
                processes = noProcesses;
                if (memoryRAM > 5000)
                    MessageBox.Show("RAM Size Exceeded Maximum RAM Requirement!", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (memoryRAM <= 0 || memoryRAM < 1000)//)
                    MessageBox.Show("RAM Size does not meet the minimum requirement", "Fixed Partition Simulator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if(noProcesses > 10)
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

            int[] partitionSizes2 = GenerateRandomPartitionSizes((int)memoryUsable, noProcesses);


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

        private int[] GenerateRandomPartitionSizes(int totalMemoryUsable, int noProcesses)
        {
            int[] partitionSizes = new int[noProcesses];

            for (int i = 0; i < noProcesses; i++)
            {
                int size = random.Next(1, totalMemoryUsable / noProcesses);
                partitionSizes[i] = size;
                totalMemoryUsable -= size;
            }

            // Ensure the last partition size is not zero
            partitionSizes[noProcesses - 1] = totalMemoryUsable;

            return partitionSizes;
        }

    }
}

