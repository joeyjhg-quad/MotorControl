namespace MotorControl
{
    partial class CustomMove
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.btn_ChangeJogMode = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.btn_Jog_Z_Minus = new System.Windows.Forms.Button();
            this.btn_DistanceSet_0001 = new System.Windows.Forms.Button();
            this.btn_Jog_Z_Plus = new System.Windows.Forms.Button();
            this.btn_DistanceSet_01 = new System.Windows.Forms.Button();
            this.btn_Jog_X_Minus = new System.Windows.Forms.Button();
            this.btn_DistanceSet_1 = new System.Windows.Forms.Button();
            this.btn_Jog_Y_Minus = new System.Windows.Forms.Button();
            this.btn_DistanceSet_001 = new System.Windows.Forms.Button();
            this.btn_Jog_X_Plus = new System.Windows.Forms.Button();
            this.btn_DistanceSet_10 = new System.Windows.Forms.Button();
            this.btn_Jog_Y_Plus = new System.Windows.Forms.Button();
            this.tb_Distance = new System.Windows.Forms.TextBox();
            this.tb_CustomMove = new System.Windows.Forms.TextBox();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.btn_ChangeJogMode);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.btn_Jog_Z_Minus);
            this.groupBox4.Controls.Add(this.btn_DistanceSet_0001);
            this.groupBox4.Controls.Add(this.btn_Jog_Z_Plus);
            this.groupBox4.Controls.Add(this.btn_DistanceSet_01);
            this.groupBox4.Controls.Add(this.btn_Jog_X_Minus);
            this.groupBox4.Controls.Add(this.btn_DistanceSet_1);
            this.groupBox4.Controls.Add(this.btn_Jog_Y_Minus);
            this.groupBox4.Controls.Add(this.btn_DistanceSet_001);
            this.groupBox4.Controls.Add(this.btn_Jog_X_Plus);
            this.groupBox4.Controls.Add(this.btn_DistanceSet_10);
            this.groupBox4.Controls.Add(this.btn_Jog_Y_Plus);
            this.groupBox4.Controls.Add(this.tb_Distance);
            this.groupBox4.Controls.Add(this.tb_CustomMove);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(396, 243);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "이동";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(41, 213);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(53, 12);
            this.label26.TabIndex = 11;
            this.label26.Text = "이동거리";
            // 
            // btn_ChangeJogMode
            // 
            this.btn_ChangeJogMode.Location = new System.Drawing.Point(71, 91);
            this.btn_ChangeJogMode.Name = "btn_ChangeJogMode";
            this.btn_ChangeJogMode.Size = new System.Drawing.Size(50, 50);
            this.btn_ChangeJogMode.TabIndex = 2;
            this.btn_ChangeJogMode.Text = "Dist";
            this.btn_ChangeJogMode.UseVisualStyleBackColor = true;
            this.btn_ChangeJogMode.Click += new System.EventHandler(this.btn_ChangeJogMode_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(283, 17);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(85, 12);
            this.label25.TabIndex = 11;
            this.label25.Text = "이동거리(mm)";
            // 
            // btn_Jog_Z_Minus
            // 
            this.btn_Jog_Z_Minus.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Jog_Z_Minus.Location = new System.Drawing.Point(197, 151);
            this.btn_Jog_Z_Minus.Name = "btn_Jog_Z_Minus";
            this.btn_Jog_Z_Minus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_Z_Minus.TabIndex = 1;
            this.btn_Jog_Z_Minus.Tag = "3,1";
            this.btn_Jog_Z_Minus.Text = "Z-";
            this.btn_Jog_Z_Minus.UseVisualStyleBackColor = true;
            this.btn_Jog_Z_Minus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_Z_Minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_Z_Minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // btn_DistanceSet_0001
            // 
            this.btn_DistanceSet_0001.Location = new System.Drawing.Point(268, 192);
            this.btn_DistanceSet_0001.Name = "btn_DistanceSet_0001";
            this.btn_DistanceSet_0001.Size = new System.Drawing.Size(113, 23);
            this.btn_DistanceSet_0001.TabIndex = 6;
            this.btn_DistanceSet_0001.Tag = "0.001";
            this.btn_DistanceSet_0001.Text = "0.001";
            this.btn_DistanceSet_0001.UseVisualStyleBackColor = true;
            this.btn_DistanceSet_0001.Click += new System.EventHandler(this.btn_DistanceSet_Click);
            // 
            // btn_Jog_Z_Plus
            // 
            this.btn_Jog_Z_Plus.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Jog_Z_Plus.Location = new System.Drawing.Point(197, 31);
            this.btn_Jog_Z_Plus.Name = "btn_Jog_Z_Plus";
            this.btn_Jog_Z_Plus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_Z_Plus.TabIndex = 1;
            this.btn_Jog_Z_Plus.Tag = "3,0";
            this.btn_Jog_Z_Plus.Text = "Z+";
            this.btn_Jog_Z_Plus.UseVisualStyleBackColor = true;
            this.btn_Jog_Z_Plus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_Z_Plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_Z_Plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // btn_DistanceSet_01
            // 
            this.btn_DistanceSet_01.Location = new System.Drawing.Point(268, 134);
            this.btn_DistanceSet_01.Name = "btn_DistanceSet_01";
            this.btn_DistanceSet_01.Size = new System.Drawing.Size(113, 23);
            this.btn_DistanceSet_01.TabIndex = 7;
            this.btn_DistanceSet_01.Tag = "0.1";
            this.btn_DistanceSet_01.Text = "0.1";
            this.btn_DistanceSet_01.UseVisualStyleBackColor = true;
            this.btn_DistanceSet_01.Click += new System.EventHandler(this.btn_DistanceSet_Click);
            // 
            // btn_Jog_X_Minus
            // 
            this.btn_Jog_X_Minus.Font = new System.Drawing.Font("굴림", 15F);
            this.btn_Jog_X_Minus.Location = new System.Drawing.Point(71, 31);
            this.btn_Jog_X_Minus.Name = "btn_Jog_X_Minus";
            this.btn_Jog_X_Minus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_X_Minus.TabIndex = 1;
            this.btn_Jog_X_Minus.Tag = "1,1";
            this.btn_Jog_X_Minus.Text = "↑";
            this.btn_Jog_X_Minus.UseVisualStyleBackColor = true;
            this.btn_Jog_X_Minus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_X_Minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_X_Minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // btn_DistanceSet_1
            // 
            this.btn_DistanceSet_1.Location = new System.Drawing.Point(268, 105);
            this.btn_DistanceSet_1.Name = "btn_DistanceSet_1";
            this.btn_DistanceSet_1.Size = new System.Drawing.Size(113, 23);
            this.btn_DistanceSet_1.TabIndex = 8;
            this.btn_DistanceSet_1.Tag = "1";
            this.btn_DistanceSet_1.Text = "1";
            this.btn_DistanceSet_1.UseVisualStyleBackColor = true;
            this.btn_DistanceSet_1.Click += new System.EventHandler(this.btn_DistanceSet_Click);
            // 
            // btn_Jog_Y_Minus
            // 
            this.btn_Jog_Y_Minus.Font = new System.Drawing.Font("굴림", 15F);
            this.btn_Jog_Y_Minus.Location = new System.Drawing.Point(11, 91);
            this.btn_Jog_Y_Minus.Name = "btn_Jog_Y_Minus";
            this.btn_Jog_Y_Minus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_Y_Minus.TabIndex = 1;
            this.btn_Jog_Y_Minus.Tag = "2,1";
            this.btn_Jog_Y_Minus.Text = "←";
            this.btn_Jog_Y_Minus.UseVisualStyleBackColor = true;
            this.btn_Jog_Y_Minus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_Y_Minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_Y_Minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // btn_DistanceSet_001
            // 
            this.btn_DistanceSet_001.Location = new System.Drawing.Point(268, 163);
            this.btn_DistanceSet_001.Name = "btn_DistanceSet_001";
            this.btn_DistanceSet_001.Size = new System.Drawing.Size(113, 23);
            this.btn_DistanceSet_001.TabIndex = 9;
            this.btn_DistanceSet_001.Tag = "0.01";
            this.btn_DistanceSet_001.Text = "0.01";
            this.btn_DistanceSet_001.UseVisualStyleBackColor = true;
            this.btn_DistanceSet_001.Click += new System.EventHandler(this.btn_DistanceSet_Click);
            // 
            // btn_Jog_X_Plus
            // 
            this.btn_Jog_X_Plus.Font = new System.Drawing.Font("굴림", 15F);
            this.btn_Jog_X_Plus.Location = new System.Drawing.Point(71, 151);
            this.btn_Jog_X_Plus.Name = "btn_Jog_X_Plus";
            this.btn_Jog_X_Plus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_X_Plus.TabIndex = 1;
            this.btn_Jog_X_Plus.Tag = "1,0";
            this.btn_Jog_X_Plus.Text = "↓";
            this.btn_Jog_X_Plus.UseVisualStyleBackColor = true;
            this.btn_Jog_X_Plus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_X_Plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_X_Plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // btn_DistanceSet_10
            // 
            this.btn_DistanceSet_10.Location = new System.Drawing.Point(268, 76);
            this.btn_DistanceSet_10.Name = "btn_DistanceSet_10";
            this.btn_DistanceSet_10.Size = new System.Drawing.Size(113, 23);
            this.btn_DistanceSet_10.TabIndex = 10;
            this.btn_DistanceSet_10.Tag = "10";
            this.btn_DistanceSet_10.Text = "10";
            this.btn_DistanceSet_10.UseVisualStyleBackColor = true;
            this.btn_DistanceSet_10.Click += new System.EventHandler(this.btn_DistanceSet_Click);
            // 
            // btn_Jog_Y_Plus
            // 
            this.btn_Jog_Y_Plus.Font = new System.Drawing.Font("굴림", 15F);
            this.btn_Jog_Y_Plus.Location = new System.Drawing.Point(131, 91);
            this.btn_Jog_Y_Plus.Name = "btn_Jog_Y_Plus";
            this.btn_Jog_Y_Plus.Size = new System.Drawing.Size(50, 50);
            this.btn_Jog_Y_Plus.TabIndex = 1;
            this.btn_Jog_Y_Plus.Tag = "2,0";
            this.btn_Jog_Y_Plus.Text = "→";
            this.btn_Jog_Y_Plus.UseVisualStyleBackColor = true;
            this.btn_Jog_Y_Plus.Click += new System.EventHandler(this.ExpansionCustomMove);
            this.btn_Jog_Y_Plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseDown);
            this.btn_Jog_Y_Plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.JogButton_MouseUp);
            // 
            // tb_Distance
            // 
            this.tb_Distance.Location = new System.Drawing.Point(285, 40);
            this.tb_Distance.Name = "tb_Distance";
            this.tb_Distance.ReadOnly = true;
            this.tb_Distance.Size = new System.Drawing.Size(76, 21);
            this.tb_Distance.TabIndex = 5;
            this.tb_Distance.Text = "1";
            this.tb_Distance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tb_CustomMove
            // 
            this.tb_CustomMove.Location = new System.Drawing.Point(116, 210);
            this.tb_CustomMove.Name = "tb_CustomMove";
            this.tb_CustomMove.Size = new System.Drawing.Size(100, 21);
            this.tb_CustomMove.TabIndex = 4;
            this.tb_CustomMove.Text = "1";
            // 
            // CustomMove
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 265);
            this.Controls.Add(this.groupBox4);
            this.Name = "CustomMove";
            this.Text = "CustomMove";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btn_ChangeJogMode;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button btn_Jog_Z_Minus;
        private System.Windows.Forms.Button btn_DistanceSet_0001;
        private System.Windows.Forms.Button btn_Jog_Z_Plus;
        private System.Windows.Forms.Button btn_DistanceSet_01;
        private System.Windows.Forms.Button btn_Jog_X_Minus;
        private System.Windows.Forms.Button btn_DistanceSet_1;
        private System.Windows.Forms.Button btn_Jog_Y_Minus;
        private System.Windows.Forms.Button btn_DistanceSet_001;
        private System.Windows.Forms.Button btn_Jog_X_Plus;
        private System.Windows.Forms.Button btn_DistanceSet_10;
        private System.Windows.Forms.Button btn_Jog_Y_Plus;
        private System.Windows.Forms.TextBox tb_Distance;
        private System.Windows.Forms.TextBox tb_CustomMove;
    }
}