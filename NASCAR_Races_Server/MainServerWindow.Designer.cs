namespace NASCAR_Races_Server
{
    partial class MainServerWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            mainPictureBox = new PictureBox();
            programTimer = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)mainPictureBox).BeginInit();
            SuspendLayout();
            // 
            // mainPictureBox
            // 
            mainPictureBox.Location = new Point(0, 0);
            mainPictureBox.Name = "mainPictureBox";
            mainPictureBox.Size = new Size(1183, 661);
            mainPictureBox.TabIndex = 0;
            mainPictureBox.TabStop = false;
            mainPictureBox.Paint += mainPictureBox_Paint;
            // 
            // button1
            // 
            button1.Location = new Point(1189, 12);
            button1.Name = "button1";
            button1.Size = new Size(189, 73);
            button1.TabIndex = 1;
            button1.Text = "Start Race";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1390, 661);
            Controls.Add(button1);
            Controls.Add(mainPictureBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainWindow";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)mainPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox mainPictureBox;
        private System.Windows.Forms.Timer programTimer;
        private Button button1;
    }
}