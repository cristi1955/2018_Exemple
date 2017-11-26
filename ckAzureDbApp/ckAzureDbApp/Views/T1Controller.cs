using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ckAzureDbApp;

namespace ckAzureDbApp.Views
{
    public class T1Controller : Controller
    {
        private ckazuredbEntities db = new ckazuredbEntities();

        // GET: T1
        public ActionResult Index()
        {
            return View(db.T1.ToList());
        }

        // GET: T1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T1 t1 = db.T1.Find(id);
            if (t1 == null)
            {
                return HttpNotFound();
            }
            return View(t1);
        }

        // GET: T1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: T1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "k1,k2,k3")] T1 t1)
        {
            if (ModelState.IsValid)
            {
                db.T1.Add(t1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(t1);
        }

        // GET: T1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T1 t1 = db.T1.Find(id);
            if (t1 == null)
            {
                return HttpNotFound();
            }
            return View(t1);
        }

        // POST: T1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "k1,k2,k3")] T1 t1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t1);
        }

        // GET: T1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T1 t1 = db.T1.Find(id);
            if (t1 == null)
            {
                return HttpNotFound();
            }
            return View(t1);
        }

        // POST: T1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T1 t1 = db.T1.Find(id);
            db.T1.Remove(t1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
