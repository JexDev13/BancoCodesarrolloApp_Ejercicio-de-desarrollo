using AutoMapper;
using BancoCodesarrolloApp_API.DTO.Cuenta;
using BancoCodesarrolloApp_API.DTO.Movimiento;
using BancoCodesarrolloApp_API.DTO.Usuario;
using BancoCodesarrolloApp_API.Models;

namespace BancoCodesarrolloApp_API.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToString("dd/MM/yyyy"));

            CreateMap<UsuarioCreacionDTO, Usuario>().ReverseMap();
            CreateMap<UsuarioActualizacionDTO, Usuario>().ReverseMap();
            CreateMap<UsuarioConsultaDTO, Usuario>().ReverseMap();
            CreateMap<Usuario, UsuarioConsultaDTO>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => $"{src.Nombre}"))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => $"{src.Apellido}"))
                .ForMember(dest => dest.CorreoElectronico, opt => opt.MapFrom(src => src.CorreoElectronico))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.Identificacion))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion));

            CreateMap<MovimientoCreacionDTO, Movimiento>().ReverseMap();
            CreateMap<Movimiento, MovimientoConsultaDTO>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.Fecha))
                .ForMember(dest => dest.TipoMovimiento, opt => opt.MapFrom(src => src.TipoMovimiento))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.CuentaId, opt => opt.MapFrom(src => src.CuentaId));

            CreateMap<CuentaCreacionDTO, Cuenta>().ReverseMap();
            CreateMap<CuentaActualizacionDTO, Cuenta>().ReverseMap();
            CreateMap<Cuenta, CuentaConsultaDTO>()
                .ForMember(dest => dest.NumeroCuenta, opt => opt.MapFrom(src => src.NumeroCuenta))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.TipoCuenta, opt => opt.MapFrom(src => src.TipoCuenta));
        }
    }
}
