using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProcurandoApartamento.Domain.Services
{
    public class ApartamentoService : IApartamentoService
    {
        protected readonly IApartamentoRepository _apartamentoRepository;

        public ApartamentoService(IApartamentoRepository apartamentoRepository)
        {
            _apartamentoRepository = apartamentoRepository;
        }

        public virtual async Task<Apartamento> Save(Apartamento apartamento)
        {
            await _apartamentoRepository.CreateOrUpdateAsync(apartamento);
            await _apartamentoRepository.SaveChangesAsync();
            return apartamento;
        }

        public virtual async Task<IPage<Apartamento>> FindAll(IPageable pageable)
        {
            var page = await _apartamentoRepository.QueryHelper()
                .GetPageAsync(pageable);
            return page;
        }

        public virtual async Task<Apartamento> FindOne(long id)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .GetOneAsync(apartamento => apartamento.Id == id);
            return result;
        }

        public virtual async Task Delete(long id)
        {
            await _apartamentoRepository.DeleteByIdAsync(id);
            await _apartamentoRepository.SaveChangesAsync();
        }

        public virtual async Task<string> GetMelhorApartamento(string[] estabelecimentos)
        {
            var Apartamentos = await _apartamentoRepository.QueryHelper()
                .GetAllFilterAsync(x => x.EstabelecimentoExiste && x.ApartamentoDisponivel
                                   && estabelecimentos.Contains(x.Estabelecimento));
            
            var ret = Apartamentos.GroupBy(x => x.Quadra).ToList();
                        
            var melhorQuadra = 0;
            var qtdImoveis = 0;

            foreach (var item in ret)
            {
                if (item.Count() >= qtdImoveis)
                {
                    melhorQuadra = item.Key;
                    qtdImoveis = item.Count();
                }

            }

            return string.Format("QUADRA: {0}", melhorQuadra.ToString());
        }
    }
}
