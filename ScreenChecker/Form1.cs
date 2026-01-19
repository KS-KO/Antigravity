using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ScreenChecker
{
    public partial class Form1 : Form
    {
        private int currentMode = 0;
        private List<Color> testColors = new List<Color>
        {
            Color.White,
            Color.Black,
            Color.Red,
            Color.Lime,
            Color.Blue,
            Color.Yellow,
            Color.Magenta,
            Color.Cyan
        };

        private Label infoLabel;

        public Form1()
        {
            // 폼 설정: 테두리 없음, 전체 화면
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.TopMost = true;

            // 안내 레이블 설정
            infoLabel = new Label();
            infoLabel.Text = "액정 진단 모드 시작\n[Space/Click] 다음 색상 | [G] 그리드 보기 | [Esc] 종료";
            infoLabel.AutoSize = true;
            infoLabel.ForeColor = Color.Gray;
            infoLabel.BackColor = Color.Transparent;
            infoLabel.Font = new Font("Malgun Gothic", 12, FontStyle.Bold);
            infoLabel.Location = new Point(20, 20);
            this.Controls.Add(infoLabel);

            // 이벤트 연결
            this.KeyDown += Form1_KeyDown;
            this.MouseDown += (s, e) => NextMode();
            
            // 더블 버퍼링 (깜빡임 방지)
            this.DoubleBuffered = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Right)
            {
                NextMode();
            }
            else if (e.KeyCode == Keys.G)
            {
                currentMode = -1; // 그리드 모드
                this.Invalidate();
            }
        }

        private void NextMode()
        {
            currentMode = (currentMode + 1) % testColors.Count;
            this.BackColor = testColors[currentMode];
            
            // 어두운 배경일 때는 텍스트 밝게
            if (this.BackColor.GetBrightness() < 0.5)
                infoLabel.ForeColor = Color.DimGray;
            else
                infoLabel.ForeColor = Color.Gray;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (currentMode == -1) // 그리드 테스트 (왜곡 확인용)
            {
                this.BackColor = Color.Black;
                using (Pen pen = new Pen(Color.DimGray, 1))
                {
                    int step = 40;
                    for (int x = 0; x < this.Width; x += step)
                        e.Graphics.DrawLine(pen, x, 0, x, this.Height);
                    for (int y = 0; y < this.Height; y += step)
                        e.Graphics.DrawLine(pen, 0, y, this.Width, y);
                }
                infoLabel.ForeColor = Color.White;
            }
        }
    }
}
