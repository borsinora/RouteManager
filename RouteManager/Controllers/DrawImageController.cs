using RouteManager.Data;
using RouteManager.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RouteManager.Controllers
{
    public class DrawImageController : Controller
    {
        DrawImageModel drawImageModel = new DrawImageModel();
        RoutesDatabaseEntities routeModel = new RoutesDatabaseEntities();

        public ActionResult Index(int? id, bool showAll = true)
        {
            int Id = 0;
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Id = id.Value;
                drawImageModel.SetRouteTypeValues(Id);
                drawImageModel.ShowAll = showAll;
                return View(drawImageModel);
            }
        }

        public ActionResult DrawImage(int id, bool showAll)
        {
            drawImageModel.SetRouteTypeValues(id);
            List<Coordinate> selectedCoordinates = drawImageModel.SelectedCoordinates;
            List<Coordinate> drawCoordinates = drawImageModel.CalculateCoordinates(selectedCoordinates);

            Bitmap b = new Bitmap(1000, 600);
            Graphics g = Graphics.FromImage(b);
            Pen p = new Pen(Color.Red);
            p.Width = 5F;

            SolidBrush sb = new SolidBrush(Color.Red);

            if (showAll) //ha az összeset
            {
                Pen p2 = new Pen(Color.Blue);
                p2.Width = 2F;

                SolidBrush sb2 = new SolidBrush(Color.Blue);
                //foreach (Route r in drawImageModel.allRoutes)
                for(int i = 0; i < drawImageModel.AllRoutes.Count; i++)
                {
                    List<Coordinate> tempCoord = drawImageModel.CalculateCoordinates(drawImageModel.AllRoutes[i].Coordinates.ToList());
                    if (i == 0)
                    {
                        DrawOnBitmap(p, tempCoord, sb, g);
                    }
                    else
                    {
                        DrawOnBitmap(p2, tempCoord, sb2, g);
                    }
                }
            }
            else
            {
                DrawOnBitmap(p, drawCoordinates, sb, g);
            }


            MemoryStream memoryStream = new MemoryStream();
            b.Save(memoryStream, ImageFormat.Jpeg);

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "image/jpg");
        }

        public void DrawOnBitmap(Pen p, List<Coordinate> coordinatesToBeDrawn, SolidBrush sb, Graphics g)
        {
            for (int i = 0; i < coordinatesToBeDrawn.Count - 1; i++)
            {
                g.DrawLine(p, (float)coordinatesToBeDrawn[i].X, (float)coordinatesToBeDrawn[i].Y, (float)coordinatesToBeDrawn[i + 1].X, (float)coordinatesToBeDrawn[i + 1].Y);
            }
            foreach (Coordinate nrc in coordinatesToBeDrawn)
            {
                FillTheDots(g, nrc, sb);
            }
        }
        public void FillTheDots(Graphics g, Coordinate c, SolidBrush sb)
        {
            g.FillEllipse(sb, (float)c.X - 5, (float)c.Y - 5, 10, 10);
        }
    }
}