using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace SevenDayRogue
{
    public class CavePoint
    {
        public int x;
        public int y;

       public  CavePoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct generation_params
    {
        public int r1_cutoff;
        public int r2_cutoff;
        public int reps;

        public generation_params(int r1, int r2, int rep)
        {
            r1_cutoff = r1;
            r2_cutoff = r2;
            reps = rep;
        }

    }

    public class CellAutoCave
    {
        public const int TILE_FLOOR = 0;
        public const int TILE_WALL = 1;
        public const int TILE_START = 2;
        public const int TILE_END = 3;


        public int[,] grid;
        public int[,] grid2;

        int fillprob = 40;
        int r1_cutoff = 5;
        int r2_cutoff = 2;
        int size_x;
        int size_y;
        generation_params gParams;
        generation_params gParams_set;

        int generations;

        Random r;


        private bool hasStart = false;
        private bool hasEnd = false;

        public CellAutoCave(int x, int y)
        {
            r = new Random();

            size_x = x;
            size_y = y;

            int argc = 10;

            generations = (argc - 4) / 3;

            gParams = new generation_params(15, 5, 3);
            gParams_set = new generation_params(r1_cutoff, r2_cutoff, 3);

            //init structs


            initmap();


            for (int i = 0; i < 3; i++)
            {
               
                generation2(true,false);
            }
            for (int i = 0; i < 2; i++)
            {
                generation2(false,false);
            }


            generation2(false, true);

        }

        public int randpick()
        {
            if (r.Next(100) < fillprob)
            {
                return TILE_WALL;
            }
            else
            {
                return TILE_FLOOR;
            }
        }

        public void initmap()
        {
            int xi, yi;
            grid = new int[size_x, size_y];
            grid2 = new int[size_x, size_y];

            List<CavePoint> pointList = new List<CavePoint>();

            for (int i = 0; i < size_x; i++)
            {
                for (int j = 0; j < size_y; j++)
                {
                    grid[i, j] = randpick();
                   

                    grid2[i, j] = TILE_WALL;
                }
            }

            //set borders = wall
            for (int i = 0; i < size_y; i++)
            {
                grid[0, i] = TILE_WALL;
                grid[1, i] = TILE_WALL;
                grid[size_x - 1, i] = TILE_WALL;
                grid[size_x - 2, i] = TILE_WALL;
            }

            for (int i = 0; i < size_x; i++)
            {
                grid[i, 0] = TILE_WALL;
                grid[i, 1] = TILE_WALL;
                grid[i, size_y - 1] = TILE_WALL;
                grid[i, size_y - 2] = TILE_WALL;
            }

            

        }

        private void AddStairs(List<CavePoint> pointList)
        {

            CavePoint p = pointList[r.Next(pointList.Count - 1)];
            pointList.Remove(p);
            AddStart(p.x,p.y);

            CavePoint p2 = pointList[r.Next(pointList.Count - 1)];
            AddEnd(p2.x, p2.y);

        }

        private void AddStart(int x, int y)
        {
            if (!hasStart)
            {
                grid2[x, y] = TILE_START;
                hasStart = true;
            }
        }

        private void AddEnd(int x, int y)
        {
            if (!hasEnd)
            {
                grid2[x, y] = TILE_END;
                hasEnd = true;
            }
        }

        //a tile becomes a wall if it was a wall and 4 or more of its nine neighbors were walls, 
        //or if it was not a wall and 5 or more neighbors were. 
        //Put more succinctly, a tile is a wall if the 3x3 region centered on it contained at least 5 walls.
        public void generation2(bool fillSpace, bool placeStairs)
        {
            int adjCount; //nearby walls
            int adjCount2; //nearby walls (in empty space)


            List<CavePoint> pointList = new List<CavePoint>();

            //traverse grid and make grid2
            for (int i = 0; i < size_x; i++)
            {
                for (int j = 0; j < size_y; j++)
                {

                    adjCount = 0;
                    adjCount2 = 0;
                    for (int ii = -1; ii <= 1; ii++)
                    {
                        for (int jj = -1; jj <= 1; jj++)
                        {
                            int tempX = i + ii;
                            int tempY = j + jj;
                            if (tempX >= 0 && tempX < size_x && tempY >= 0 && tempY < size_y)
                            {
                                if (grid[i + ii, j + jj] != TILE_FLOOR)
                                    adjCount++;
                            }
                        }
                    }


                    if(grid2[i,j] == TILE_START || grid2[i,j] == TILE_END)
                    {

                    
                    }
                    else if (adjCount >= 5)
                    {
                        grid2[i, j] = TILE_WALL;
                    }
                    else
                    {
                        pointList.Add(new CavePoint(i, j));
                        grid2[i, j] = TILE_FLOOR;
                    }

                    if (fillSpace)
                    {
                        for (int ii = -2; ii <= 2; ii++)
                        {
                            for (int jj = -2; jj <= 2; jj++)
                            {
                                int tempX = i + ii;
                                int tempY = j + jj;
                                if (tempX >= 0 && tempX < size_x && tempY >= 0 && tempY < size_y)
                                {
                                    if (grid[i + ii, j + jj] != TILE_FLOOR)
                                        adjCount2++;
                                }
                            }
                        }

                        if (adjCount2 <= 2)
                        {
                            grid2[i, j] = TILE_WALL;
                        }

                    }

                }
            }

            if (placeStairs)
            {
                AddStairs(pointList);
            }


            for (int i = 0; i < size_x; i++)
            {
                for (int j = 0; j < size_y; j++)
                {
                    grid[i, j] = grid2[i, j];
                }
            }


           
        }

        //Not called?
        public void generation()
        {
            int xi, yi, ii, jj;

            for (yi = 1; yi < size_y - 1; yi++)
            {
                for (xi = 1; xi < size_x - 1; xi++)
                {
                    int adjcount_r1 = 0,
                    adjcount_r2 = 0;

                    for (ii = -1; ii <= 1; ii++)
                    {
                        for (jj = -1; jj <= 1; jj++)
                        {
                            if (grid[xi + ii, yi + jj] != TILE_FLOOR)
                                adjcount_r1++;
                        }
                    }

                    for (ii = yi - 2; ii <= yi + 2; ii++)
                    {
                        for (jj = xi - 2; jj <= xi + 2; jj++)
                        {
                            if (Math.Abs(ii - yi) == 2 && (Math.Abs(jj - xi) == 2))
                                continue;
                            if (ii < 0 || jj < 0 || ii >= size_y || jj >= size_x)
                                continue;
                            if (grid[jj, ii] != TILE_FLOOR)
                                adjcount_r2++;
                        }
                    }

                    //fix this
                    if (adjcount_r1 >= gParams.r1_cutoff || adjcount_r2 <= gParams.r2_cutoff)
                        grid2[xi, yi] = TILE_WALL;
                    else
                        grid2[xi, yi] = TILE_FLOOR;


                }
                for (yi = 1; yi < size_y - 1; yi++)
                {
                    for (xi = 1; xi < size_x - 1; xi++)
                    {
                        grid[xi, yi] = grid2[xi, yi];
                    }
                }

            }

        }

      
        private double dist(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        


    }
}
