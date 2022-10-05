using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarGame
{
    sealed public partial class Game_terrain : Form
    {
        // Variables
        private bool is_resized = false;
        private int terrain_speed = 2;
        List<PictureBox> my_terrain;
        List<PictureBox> my_obstacles;
        private int terrain_width = 744;
        private int terrain_height = 631;
        private int level = 1;
        private int increase_level = 0;
        private bool movement_locked = false;
        private int coints = 0;

        // Initialise form
        public Game_terrain()
        {
            InitializeComponent();
            my_terrain = new List<PictureBox>();

            // Add all the moving terrain
            my_terrain.Add(pictureBox3);
            my_terrain.Add(pictureBox4);
            my_terrain.Add(pictureBox5);
            my_terrain.Add(pictureBox6);
            my_terrain.Add(pictureBox7);
            my_terrain.Add(pictureBox8);
            my_terrain.Add(pictureBox9);
            my_terrain.Add(pictureBox10);
            my_terrain.Add(pictureBox11);
            my_terrain.Add(pictureBox12);

            // Set player to start in the middle of the map
            player.Location = new Point((terrain_width / 2) - (player.Width / 2), terrain_height - player.Height - 100);

            // Spawn enemies
            my_obstacles = new List<PictureBox>();
            my_obstacles.Add(pictureBox15);
            my_obstacles.Add(pictureBox16);
            var random_location = new Random();
            foreach (var enemy in my_obstacles)
            {
                enemy.Location = new Point(random_location.Next(10, terrain_width - pictureBox15.Width - 10),
                random_location.Next(20, terrain_height / 2 - 60));
            }

            // Hide game-over display
            label6.Visible = false;

            // Display coin
            pictureBox17.Location = new Point(random_location.Next(10, terrain_width - 100),
                random_location.Next(20, terrain_height / 2 - 100));
        }

        // Resize button
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (is_resized == false)
            {
                // Resize game and center it
                this.Width = this.Width * 2;
                terrain_width = this.Width;
                is_resized = true;
                this.CenterToScreen();

                // Move buttons
                pictureBox1.Location = new Point(pictureBox1.Location.X * 2 + 45, pictureBox1.Location.Y);
                pictureBox2.Location = new Point(pictureBox2.Location.X * 2 + 85, pictureBox2.Location.Y);
                foreach ( var element in my_terrain)
                {
                    element.Location = new Point(element.Location.X * 2, element.Location.Y);
                }
                pictureBox13.Location = new Point(pictureBox13.Location.X * 2 + 30, pictureBox13.Location.Y);

                // Move HUD
                label2.Location = new Point(label2.Location.X * 2, label2.Location.Y);
                label3.Location = new Point(label3.Location.X * 2 + 35, label3.Location.Y);
                label4.Location = new Point(label4.Location.X * 2, label4.Location.Y);
                label5.Location = new Point(label5.Location.X * 2, label5.Location.Y);

                // Move player, enemies, coin
                player.Location = new Point(player.Location.X * 2, player.Location.Y);
                pictureBox15.Location = new Point(pictureBox15.Location.X * 2, pictureBox15.Location.Y);
                pictureBox16.Location = new Point(pictureBox16.Location.X * 2, pictureBox16.Location.Y);
                pictureBox17.Location = new Point(pictureBox17.Location.X * 2, pictureBox17.Location.Y);
            }
            else if (is_resized == true)
            {
                // Resize game and center it
                this.Width = this.Width / 2;
                terrain_width = this.Width;
                is_resized = false;
                this.CenterToScreen();

                // Move buttons
                pictureBox1.Location = new Point((pictureBox1.Location.X - 45) / 2, pictureBox1.Location.Y);
                pictureBox2.Location = new Point((pictureBox2.Location.X - 85) / 2, pictureBox2.Location.Y);
                foreach (var element in my_terrain)
                {
                    element.Location = new Point(element.Location.X / 2, element.Location.Y);
                }
                pictureBox13.Location = new Point((pictureBox13.Location.X - 30) / 2, pictureBox13.Location.Y);

                // Move HUD
                label2.Location = new Point(label2.Location.X / 2, label2.Location.Y);
                label3.Location = new Point((label3.Location.X - 35) / 2, label3.Location.Y);
                label4.Location = new Point(label4.Location.X / 2, label4.Location.Y);
                label5.Location = new Point(label5.Location.X / 2, label5.Location.Y);


                // Move player, enemies, coin
                player.Location = new Point(player.Location.X / 2, player.Location.Y);
                pictureBox15.Location = new Point(pictureBox15.Location.X / 2, pictureBox15.Location.Y);
                pictureBox16.Location = new Point(pictureBox16.Location.X / 2, pictureBox16.Location.Y);
                pictureBox17.Location = new Point(pictureBox17.Location.X / 2, pictureBox17.Location.Y);
            }
        }

        // Close button
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        // Call terrain movement every tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Move obstacles and terain
            line_speedUp(terrain_speed);
            obstacle_speedUp(terrain_speed);
            coin_speedUp();
            coin_display();

            // Periodically increase difficulty
            increase_level++;
            if (increase_level == 500)
            {
                level++;
                increase_level = 0;
                if (terrain_speed < 18)
                    terrain_speed += 2;
            }

            // Display coins, speed and level
            label3.Text = level.ToString();
            label5.Text = terrain_speed.ToString() + " km/h";
            label8.Text = coints.ToString();
        }

        // Move terrain
        private void line_speedUp(int speed)
        {
            // Check if the game is still running
            if (movement_locked == true)
                return;

            // Move everything down
            foreach (var element in my_terrain)
            {
                element.Top += speed;

                // If the element leaves the border, put it back up
                if(element.Location.Y > 631 - element.Height) {
                    element.Location = new Point(element.Location.X, element.Location.Y - 650);
                }
            }
        }

        // Player controls
        private void Game_terrain_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the game is still running
            if (movement_locked == true)
                return;

            // Speed UP / DOWN and go LEFT / RIGHT
            if (e.KeyCode == Keys.W)
            {
                if(terrain_speed <= 16)
                    terrain_speed += 2;
            }
            else if(e.KeyCode == Keys.A)
            {
                if(player.Location.X > 10)
                    player.Location = new Point(player.Location.X - 10, player.Location.Y);
            }
            else if (e.KeyCode == Keys.D)
            {
                if (player.Location.X < terrain_width - player.Width-28)
                    player.Location = new Point(player.Location.X + 10, player.Location.Y);
            }
            else if (e.KeyCode == Keys.S)
            {
                if(terrain_speed > 3 && terrain_speed > level + 3)
                    terrain_speed -= 2;
            }
        }

        // Move the obstacles
        private void obstacle_speedUp(int speed)
        {
            // Check if the game is still running
            if (movement_locked == true)
                return;

            foreach (var obstacle in my_obstacles)
            {
                obstacle.Top += speed / 2;
                // Check if we are still on the map
                if(obstacle.Top > terrain_width - obstacle.Height)
                {
                    var random_location = new Random();
                    obstacle.Location = new Point(random_location.Next(10, terrain_width - pictureBox15.Width - 10), 50);
                }

                // Check collision with player
                if(player.Bounds.IntersectsWith(obstacle.Bounds))
                {
                    timer1.Enabled = false;
                    game_over();
                }
            }
        }

        // Game over screen
        private void game_over()
        {
            // Stop terrain from moving
            terrain_speed = 0;
            movement_locked = true;

            // Display game-over display
            label6.Visible = true;

        }

        // Move coin
        private void coin_speedUp()
        {
            var random_location = new Random();
            pictureBox17.Top += 2;
            if (pictureBox17.Location.Y > terrain_height - pictureBox17.Height) 
                pictureBox17.Location = new Point(random_location.Next(10, terrain_width - 100), 10);
        }

        // Display coin
        private void coin_display()
        {
            // Get random position
            var random_position = new Random();

            // Check player collision
            if (player.Bounds.IntersectsWith(pictureBox17.Bounds))
            {
                // Display coin
                var random_location = new Random();
                pictureBox17.Location = new Point(random_location.Next(10, terrain_width - 100), random_position.Next(20, terrain_height / 2 - 100));
                coints++;
            }
        }
    }
}
