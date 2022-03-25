﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using WebApplication.Service;
using WebApplication.Model;

namespace WebApplication1.Controllers
{
    public class ProductController : ApiController
    {
     

        static List<Product> products = new List<Product>();
        static List<Product> sqlProducts = new List<Product>();

        public static string connectionString = @"Data Source=DESKTOP-KKL4FN6\SQLEXPRESS;Initial Catalog = vjezba; Integrated Security = True";

        [HttpGet]
        [Route("webapi/getbyidmultilayer")]

        public HttpResponseMessage GetProduct(Guid id)
        {
            ProductService productService = new ProductService();
            List<ProductModel> modelProducts = new List<ProductModel>();

            modelProducts=productService.GetProduct(id);

            if(modelProducts==null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, modelProducts);
            }


        }




        [HttpGet]
        [Route("webapi/getsqlproduct")]
        public HttpResponseMessage GetTableContent()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Product", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product();
                    product.Price = reader.GetDecimal(0);
                    product.Name = reader.GetString(1);
                    product.Id = reader.GetGuid(2);
                    product.Stock = reader.GetInt32(3);
                    product.CountryOfOrigin = reader.GetString(4);
                    sqlProducts.Add(product);

                }
            }
         //connection.Close();
         return Request.CreateResponse(HttpStatusCode.OK, sqlProducts);

        }

        [HttpGet]
        [Route("webapi/getsqlhaving")]

        public HttpResponseMessage GetProductPriceOverX(decimal price)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand($"SELECT * FROM Product GROUP BY Price HAVING Price > {price}'", connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
            }
            return Request.CreateResponse(HttpStatusCode.OK, sqlProducts);



        }

        [HttpGet]
        [Route("webapi/getsqlproductbyid")]

        public HttpResponseMessage GetProductById(Guid id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand($"SELECT * FROM Product WHERE Id ='{id}'", connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    Product product = new Product();
                    product.Price = reader.GetDecimal(0);
                    product.Name = reader.GetString(1);
                    product.Id = reader.GetGuid(2);
                    product.Stock = reader.GetInt32(3);
                    product.CountryOfOrigin = reader.GetString(4);
                    sqlProducts.Add(product);

                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, sqlProducts);
        }

        // GET: api/Product
        [HttpGet]

        [Route("webapi/getproduct")]
        public HttpResponseMessage FindProductInfo( Guid productId)
        {
            Product foundProduct = products.Find(product => product.Id == productId);
            if (productId == foundProduct.Id)
            {
                return Request.CreateResponse(HttpStatusCode.Found, foundProduct);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"The item with the id{productId} is not in our stock!");
            }
        }

        // GET: api/Product/5
        [HttpGet]

        [Route("products/under50")]
        public HttpResponseMessage FindProductsUnder50Euro()
        {
            List<Product> cheapProducts = new List<Product>();
            foreach(Product product in products)
            {
                if(product.Price<50)
                {
                    cheapProducts.Add(product);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, cheapProducts);
        }

        // POST: api/Prnoduct


        [HttpPost]

        [Route("webapi/populatedataset")]

        //primjer data adaptera
        //preko data adaptera mozemo vidjeti sve podatke u tablici
        public HttpResponseMessage PopulateDataset()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string adapterString = "SELECT * FROM Product";
                SqlDataAdapter myAdapter = new SqlDataAdapter(adapterString, connectionString);
                DataTable products = new DataTable();
                myAdapter.Fill(products);
                return Request.CreateResponse(HttpStatusCode.OK, products.Rows);
                // mozes returnat products.Rows
            }
       
         }

        [HttpPost]

        [Route("product/addproduct")]
        public HttpResponseMessage AddNewProduct(Product newProduct)
        {
            if(newProduct!=null)
            {
                products.Add(newProduct);
                return Request.CreateResponse(HttpStatusCode.OK, $"A new product {newProduct.Name} has been added");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"You haven't entered anything!");
            }


        }

        [HttpPost]

        
        [Route("webapi/insertproductsql")]

        public HttpResponseMessage InsertProduct(Product product)

        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand($"INSERT INTO Product (Price, Title, Stock, CountryOfOrigin) VALUES ('{product.Price}','{product.Name}', {product.Stock}, '{product.CountryOfOrigin}');", connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

            }
            return Request.CreateResponse(HttpStatusCode.OK, "Product has been added to the database!");
            }

        [HttpPost]
        [Route("webapi/insertproductadapter")]

        public HttpResponseMessage InserNewProductWithAdapter(Product product)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection connection = new SqlConnection(connectionString);
            using(connection)
            { 
                connection.Open();
                string newProductCommand = $"INSERT INTO Product (Price, Title, Stock, CountryOfOrigin) VALUES ('{product.Price}','{product.Name}', {product.Stock}, '{product.CountryOfOrigin}');";
            adapter.InsertCommand= new SqlCommand(newProductCommand, connection);

                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();

            }
            return Request.CreateResponse(HttpStatusCode.OK, "You have inserted a new product!");
        }
        

        // PUT: api/Product/5
        //executenonquery
        [HttpPut]

        [Route("product/sqlupdateprice")]

        public HttpResponseMessage UpdatePrice(Guid id)
        {

            //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-KKL4FN6\SQLEXPRESS;Initial Catalog = vjezba; Integrated Security = True");
            //preko dataadaptera
            //using (SqlCommand command = new SqlCommand($"UPDATE Product SET Price='89.99' WHERE Id ='{id}'", connection))
            //{
            //    connection.Open();
            //    SqlDataAdapter adapter = new SqlDataAdapter(command, connection);
            //    //SqlDataReader reader = command.ExecuteReader();
            //}

            //return Request.CreateResponse(HttpStatusCode.OK, "The price has been updated!");

            string command = $"SELECT * FROM Product";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter(command, connection);

            DataSet ds  = new DataSet();

            adapter.Fill(ds, "Product"); 

            DataTable dataTable = ds.Tables["Product"];

            dataTable.Rows[0]["Title"] = "Headphones";

            string sql = $"UPDATE Product SET Price='89.99' WHERE Id ='{id}'";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            sqlDataAdapter.UpdateCommand=cmd;
            adapter.Update(ds, "Product");
            return Request.CreateResponse(HttpStatusCode.OK, "The product has been updated!");



        }

        [HttpPut]

        [Route("product/updateprice")]
        public HttpResponseMessage UpdateProductPrice(Guid productId, decimal newPrice)
        {
            Product updatedProduct = products.Find(product => product.Id == productId);
            if (productId==updatedProduct.Id)
            {
                decimal oldPrice=updatedProduct.Price;
                updatedProduct.Price = newPrice;
                return Request.CreateResponse(HttpStatusCode.OK, $"The product price has been updated from {oldPrice} to {newPrice}");
               
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "The product that you're looking for isn't here!");

            }
        }

        // DELETE: api/Product/5
        public HttpResponseMessage Delete(Guid productId)
        {
            Product soonToBeDeletedProduct = products.Find(product => product.Id == productId);
            if (productId == soonToBeDeletedProduct.Id)
            {
                products.Remove(soonToBeDeletedProduct);
                return Request.CreateResponse(HttpStatusCode.OK, $"The product with the id {productId} has been deleted!");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"The item with the id{productId} is not in our stock!");
            }


        }

        [HttpDelete]

     [Route("webapi/deletesql")] //?id=725018C6-2B1B-41F8-A3E8-17A55C709535

     //ne mozes obrisati tablice koje su povezane s drugom tablicom s foreign key osim sa cascadeom
        public HttpResponseMessage DeleteSql(Guid id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand command = new SqlCommand($"DELETE FROM Product WHERE Id ='{id}'", connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    return Request.CreateResponse(HttpStatusCode.OK, "The item has been deleted!");
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "The item doesn't exist");
            }

            


        }



    }
}