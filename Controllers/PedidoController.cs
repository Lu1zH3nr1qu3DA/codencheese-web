﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebHamburgueria.Models;
using WebHamburgueria.ViewModels;

namespace WebHamburgueria.Controllers
{
    public class PedidoController : Controller
    {
        private dbhamburgueriaEntities db = new dbhamburgueriaEntities();

        // GET: Pedido
        public ActionResult Index()
        {
            return View(db.Pedido.ToList());
        }

        // GET: Pedido/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // GET: Pedido/Create
        public ActionResult Create()
        {
            var produtos = db.Produto.Select(p => new DTOs.ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco
            }).ToList();

            // Armazene a lista no ViewBag para uso na view
            ViewBag.ProdutoData = produtos;

            // Inicializa o ViewModel do Pedido com uma lista vazia de itens
            var model = new PedidoViewModel
            {
                Itens = new List<ItensPedidoViewModel>()
            };

            return View(model);
        }

        // POST: Pedido/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Se o CPF do usuário for nulo, defina-o como uma string vazia
                if (model.CpfUsuario == null) model.CpfUsuario = "";

                // Mapeie o ViewModel para as entidades do banco
                var pedido = new Pedido
                {
                    CpfUsuario = model.CpfUsuario,
                    DataPedido = DateTime.Now,
                    Status = "A",
                    MetPag = model.MetPag,
                    // Se você quiser calcular o Total automaticamente,
                    // some os PrecoProduto dos itens:
                    Total = model.Itens.Sum(i => Convert.ToDecimal(i.PrecoProduto, new CultureInfo("en-US")))
                };

                // Salve o Pedido
                db.Pedido.Add(pedido);
                db.SaveChanges();

                // Adicione os pontos ao usuário
                var usuario = db.Usuario.SingleOrDefault(o => o.Cpf == model.CpfUsuario);
                if (usuario != null)
                {
                    usuario.Pontos += Convert.ToInt32(pedido.Total);
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Salve os Itens
                foreach (var itemVm in model.Itens)
                {
                    var produto = db.Produto.Find(itemVm.IdProduto);
                    if (produto != null)
                    {
                        var item = new ItensPedido
                        {
                            IdPedido = pedido.Id,
                            IdProduto = produto.Id,
                            NomeProduto = produto.Nome,
                            PrecoProduto = produto.Preco
                        };
                        db.ItensPedido.Add(item);
                    }
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Se o ModelState não estiver válido, recarregue a lista de produtos
            var produtos = db.Produto
                                   .Select(p => new DTOs.ProdutoDTO
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome,
                                       Preco = p.Preco
                                   })
                                   .ToList();
            ViewBag.ProdutoData = produtos;

            return View(model);
        }


        // GET: Pedido/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PedidoViewModel pedidovm)
        {
            var pedido = db.Pedido.Find(pedidovm.Id);

            pedido.CpfUsuario = pedidovm.CpfUsuario;
            pedido.Status = pedidovm.Status;
            pedido.MetPag = pedidovm.MetPag;
            pedido.Total = pedidovm.Itens.Sum(i => Convert.ToDecimal(i.PrecoProduto));
            pedido.DataPedido = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pedido);
        }

        // GET: Pedido/Edit/5
        public ActionResult Finish(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Finish([Bind(Include = "Id,Status")] PedidoViewModel pedidovm)
        {
            var pedido = db.Pedido.Find(pedidovm.Id);

            pedido.Status = "F";

            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pedidovm);
        }

        // GET: Pedido/Edit/5
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel([Bind(Include = "Id,Status")] PedidoViewModel pedidovm)
        {
            var pedido = db.Pedido.Find(pedidovm.Id);
            pedido.Status = "C";

            var usuario = db.Usuario.SingleOrDefault(o => o.Cpf == pedido.CpfUsuario);
            if (usuario != null && usuario.Pontos > 0)
            {
                usuario.Pontos -= Convert.ToInt32(pedido.Total);
            }

            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(pedidovm);
        }

        // GET: Pedido/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pedido pedido = db.Pedido.Find(id);
            db.Pedido.Remove(pedido);
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
