namespace DiaryJournal.Net
{
    partial class FormSelectDesign
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOK = new Button();
            btnCancel = new Button();
            groupBox3 = new GroupBox();
            label2 = new Label();
            label1 = new Label();
            radioDesign2 = new RadioButton();
            radioDesign1 = new RadioButton();
            label3 = new Label();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(188, 222);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 15;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(283, 222);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(radioDesign2);
            groupBox3.Controls.Add(radioDesign1);
            groupBox3.Location = new Point(12, 12);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(359, 204);
            groupBox3.TabIndex = 16;
            groupBox3.TabStop = false;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(22, 141);
            label2.Name = "label2";
            label2.Size = new Size(331, 20);
            label2.TabIndex = 3;
            label2.Text = "old 1990s Dos file manager navigation design.";
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(22, 54);
            label1.Name = "label1";
            label1.Size = new Size(331, 52);
            label1.TabIndex = 2;
            label1.Text = "tree view design which uses intelligent lazy loading like windows explorer.";
            // 
            // radioDesign2
            // 
            radioDesign2.AutoSize = true;
            radioDesign2.Font = new Font("Segoe UI", 14.25F);
            radioDesign2.Location = new Point(6, 109);
            radioDesign2.Name = "radioDesign2";
            radioDesign2.Size = new Size(211, 29);
            radioDesign2.TabIndex = 1;
            radioDesign2.Text = "journal form design 2";
            radioDesign2.UseVisualStyleBackColor = true;
            // 
            // radioDesign1
            // 
            radioDesign1.AutoSize = true;
            radioDesign1.Checked = true;
            radioDesign1.Font = new Font("Segoe UI", 14.25F);
            radioDesign1.Location = new Point(6, 22);
            radioDesign1.Name = "radioDesign1";
            radioDesign1.Size = new Size(211, 29);
            radioDesign1.TabIndex = 0;
            radioDesign1.TabStop = true;
            radioDesign1.Text = "journal form design 1";
            radioDesign1.UseVisualStyleBackColor = true;
            radioDesign1.CheckedChanged += radioDesign1_CheckedChanged;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(6, 161);
            label3.Name = "label3";
            label3.Size = new Size(347, 40);
            label3.TabIndex = 4;
            label3.Text = "100,000 or more nodes cannot be loaded as it causes memory overflow and application or windows crashes.";
            // 
            // FormSelectDesign
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(384, 261);
            ControlBox = false;
            Controls.Add(groupBox3);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSelectDesign";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "select journal form design";
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOK;
        private Button btnCancel;
        private GroupBox groupBox3;
        public RadioButton radioDesign2;
        public RadioButton radioDesign1;
        private Label label2;
        private Label label1;
        private Label label3;
    }
}