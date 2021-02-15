using System;
using System.Collections.Generic;
using System.Linq;
using MetWorkingGeo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MetWorkingGeo.API.Data;

namespace MetWorkingGeo.API.Controllers
{
	[Produces("application/json")]
	[Route("api/geolocalizacao")]
	public class GeolocalizacaoController : Controller
	{
		private readonly DBGeolocalizacaoMongodb mongoContext;

		public GeolocalizacaoController()
		{
			mongoContext = new DBGeolocalizacaoMongodb();
		}

		[HttpGet]
		public IEnumerable<Geolocalizacao> GetAll()
		{
			List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

			lLstGeolocalizacao = mongoContext.dbGeolocalizacao.Find(x => true).ToList();

			return lLstGeolocalizacao;
		}

		[HttpGet("{idUser}", Name = "GetById")]
		
		public IEnumerable<Geolocalizacao> GetById(Guid idUser)
		{
			List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

			lLstGeolocalizacao = mongoContext.dbGeolocalizacao.Find(x => x.idUser.Equals(idUser)).ToList();

			return lLstGeolocalizacao;
		}

		[HttpPost]
		public void Post([FromBody]Geolocalizacao pGeo)
		{
			mongoContext.dbGeolocalizacao.InsertOne(pGeo);
		}

		//[HttpPut]
		//public void Put(Guid idUser)
		//{
		//	Geolocalizacao lObjGeolocalizacao = mongoContext.dbGeolocalizacao.Find(x => x.idUser.Equals(idUser)).FirstOrDefault();

		//	lObjGeolocalizacao.latitude = 0;

		//	mongoContext.dbGeolocalizacao.ReplaceOne(x=> x.id == lObjGeolocalizacao.id, lObjGeolocalizacao);
		//}

		//[HttpDelete]
		//public void Delete(Guid idUser)
		//{
		//	mongoContext.dbGeolocalizacao.DeleteOne(x => x.id == idUser);
		//}
	}
}