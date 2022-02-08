using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatable.Models;
using datatable.Models.ViewModels;
namespace datatable.Controllers
{
    public class PersonasController : Controller
    {
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;
        private IncentivosEntities8 _db = new IncentivosEntities8();


        // GET: Personas
        public ActionResult Index()
        {
            return View();
        }

       

        //server
        [HttpPost]
        public ActionResult Json()
        {

            List<TablePersonasViewModel> lst = new List<TablePersonasViewModel>();
            List<Regalos> regalos = new List<Regalos>();

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault().ToString();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 100;
            using (IncentivosEntities8 db =new IncentivosEntities8())
            {
                inicializarentrega();
                lst = (from d in db.Colaboradores
                    
                    

                           //where d.Nombre.Contains(searchValue)

                       select new TablePersonasViewModel

                       {
                           Cla_trab = d.Cla_trab,
                           Nombre = d.Nombre,
                           Departamento = d.Departamento,
                          
                         

             

                       }).ToList();
                recordsTotal = lst.Count;
                lst = lst.Skip(skip).Take(pageSize).ToList();

              
            }
             var ganador = lst.OrderBy(g => new Guid()).Take(100);

            List<TablePersonasViewModel> listaconpremio = new List<TablePersonasViewModel>();
            int contadorp = 0;
            foreach (var item in ganador)
            {
                TablePersonasViewModel pg = new TablePersonasViewModel();
                pg.Cla_trab = item.Cla_trab;
                pg.Nombre = item.Nombre;
                pg.Departamento = item.Departamento;
                //pg.Regalo = _db.Regalos.Where(x => x.Regalo.Equals("playstation7")).FirstOrDefault().Regalo;
                int almaceno = obtieneregalo();
                var qqq = _db.Regalos.Where(x => x.Id_regalo.Equals(almaceno)).FirstOrDefault();
                pg.Regalo = qqq.Regalo;
                contadorp++;
                listaconpremio.Add(pg);

                qqq.Entregado = "1";

                _db.Entry(qqq).State = System.Data.EntityState.Modified;
                _db.SaveChanges();

                


            }
            
            //return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = ganador });
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaconpremio });

        }
        //funcionar un metodo
        public void inicializarentrega()
        {
            var lst1 = _db.Regalos.ToList();
            foreach (var item in lst1)

            {
                item.Entregado = "0";


                _db.Entry(item).State = System.Data.EntityState.Modified;
                _db.SaveChanges();

            }

        }


        public void Guardarganadores(List <TablePersonasViewModel> lista)
        {

            foreach (var item in lista)
            {
                Ganadores g = new Ganadores();
                g.Clab_Trab = item.Cla_trab;
                g.Nombre = item.Nombre;
                g.Regalo = item.Regalo;
                g.Departamento = item.Departamento;

                _db.Ganadores.Add(g);
                _db.SaveChanges();
            }
        }


       
        public int obtieneregalo()
        {
           var lst = _db.Regalos.Where(x=>x.Entregado.Equals("0")).OrderBy(g => Guid.NewGuid()).Take(1).FirstOrDefault();
            //var lst = (from d in _db.Regalos
            
            return  lst.Id_regalo;

        }
    }
}