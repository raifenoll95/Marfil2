USE [marfilestable]
GO
/****** Object:  Table [dbo].[Acabados]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Acabados](
	[id] [varchar](2) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[descripcion2] [varchar](40) NULL,
	[descripcionabreviada] [varchar](20) NULL,
 CONSTRAINT [PK_Acabados] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Acreedores]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Acreedores](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fkidiomas] [varchar](3) NOT NULL,
	[fkfamiliaacreedor] [varchar](3) NULL,
	[fkzonaacreedor] [varchar](3) NULL,
	[fktipoempresa] [varchar](3) NULL,
	[fkunidadnegocio] [varchar](3) NULL,
	[fkincoterm] [varchar](3) NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[fkmonedas] [int] NOT NULL,
	[fkregimeniva] [varchar](5) NOT NULL,
	[fkgruposiva] [varchar](4) NULL,
	[criterioiva] [int] NOT NULL,
	[fktiposretencion] [varchar](4) NULL,
	[fktransportistahabitual] [varchar](15) NULL,
	[tipoportes] [int] NULL,
	[cuentatesoreria] [varchar](15) NULL,
	[fkformaspago] [int] NOT NULL,
	[descuentoprontopago] [float] NULL,
	[descuentocomercial] [float] NULL,
	[diafijopago1] [int] NULL,
	[diafijopago2] [int] NULL,
	[periodonopagodesde] [varchar](5) NULL,
	[periodonopagohasta] [varchar](5) NULL,
	[notas] [text] NULL,
	[tarifa] [varchar](50) NULL,
 CONSTRAINT [PK_Acreedores] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Administrador]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Administrador](
	[password] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Administrador] PRIMARY KEY CLUSTERED 
(
	[password] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Agentes]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Agentes](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fktipoirpf] [varchar](4) NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fkformapago] [int] NULL,
	[porcentajecomision] [float] NULL,
	[comisionporm2] [float] NULL,
	[comisionporm3] [float] NULL,
	[porcentajeincrementosobreptb] [float] NULL,
	[primaincrementosobreptb] [float] NULL,
	[porcentajedecrementosobreptb] [float] NULL,
	[primadecrementosobreptb] [float] NULL,
 CONSTRAINT [PK_Agentes] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Albaranes]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Albaranes](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fkejercicio] [int] NOT NULL CONSTRAINT [DF__Albaranes__fkeje__2AF556D4]  DEFAULT ((1)),
	[fkseries] [varchar](3) NOT NULL,
	[identificadorsegmento] [varchar](12) NOT NULL,
	[referencia] [varchar](30) NULL,
	[fechadocumento] [datetime] NULL,
	[fechavalidez] [datetime] NULL,
	[fechaentrega] [datetime] NULL,
	[fecharevision] [datetime] NULL,
	[fkalmacen] [int] NULL,
	[fkclientes] [varchar](15) NULL,
	[nombrecliente] [varchar](200) NULL,
	[clientedireccion] [varchar](100) NULL,
	[clientepoblacion] [varchar](100) NULL,
	[clientecp] [varchar](10) NULL,
	[clientepais] [varchar](50) NULL,
	[clienteprovincia] [varchar](50) NULL,
	[clientetelefono] [varchar](50) NULL,
	[clientefax] [varchar](15) NULL,
	[clienteemail] [varchar](200) NULL,
	[clientenif] [varchar](15) NULL,
	[fkformaspago] [int] NULL,
	[fkagentes] [varchar](15) NULL,
	[fkcomerciales] [varchar](15) NULL,
	[comisionagente] [float] NULL,
	[comisioncomercial] [float] NULL,
	[fkmonedas] [int] NULL,
	[cambioadicional] [float] NULL,
	[importebruto] [float] NULL,
	[importebaseimponible] [float] NULL,
	[importeportes] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[importetotaldoc] [float] NULL,
	[importetotalmonedabase] [float] NULL,
	[notas] [text] NULL,
	[fkestados] [varchar](10) NULL,
	[fkobras] [varchar](15) NULL,
	[incoterm] [varchar](3) NULL,
	[descripcionincoterm] [varchar](30) NULL,
	[peso] [int] NULL,
	[confianza] [int] NULL,
	[costemateriales] [float] NULL,
	[tiempooficinatecnica] [float] NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fktransportista] [varchar](15) NULL,
	[tipocambio] [float] NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[unidadnegocio] [varchar](3) NULL,
	[referenciadocumento] [varchar](30) NULL,
	[fkbancosmandatos] [varchar](3) NULL,
	[cartacredito] [varchar](25) NULL,
	[vencimientocartacredito] [datetime] NULL,
	[contenedores] [int] NULL,
	[fkcuentastesoreria] [varchar](15) NULL,
	[numerodocumentoproveedor] [varchar](30) NULL,
	[fechadocumentoproveedor] [datetime] NULL,
	[fkclientesreserva] [varchar](15) NULL,
	[tipoalbaran] [int] NOT NULL,
	[fkmotivosdevolucion] [varchar](3) NULL,
	[nombretransportista] [varchar](40) NULL,
	[conductor] [varchar](20) NULL,
	[matricula] [varchar](12) NULL,
	[bultos] [int] NULL,
	[pesoneto] [float] NULL,
	[pesobruto] [float] NULL,
	[volumen] [float] NULL,
	[envio] [text] NULL,
	[fkoperarios] [varchar](15) NULL,
	[fkoperadortransporte] [varchar](15) NULL,
	[fkzonas] [varchar](3) NULL,
	[fkdireccionfacturacion] [int] NULL,
	[fkcriteriosagrupacion] [varchar](4) NULL,
	[tipoportes] [int] NULL,
	[costeportes] [float] NULL,
	[fkusuarioalta] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Albaranes__fkusu__2724C5F0]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechaalta] [datetime] NOT NULL CONSTRAINT [DF__Albaranes__fecha__2818EA29]  DEFAULT (getdate()),
	[fkusuariomodificacion] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Albaranes__fkusu__290D0E62]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechamodificacion] [datetime] NOT NULL CONSTRAINT [DF__Albaranes__fecha__2A01329B]  DEFAULT (getdate()),
	[fkcarpetas] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Albaranes_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlbaranesLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AlbaranesLin](
	[empresa] [varchar](4) NOT NULL,
	[fkalbaranes] [int] NOT NULL CONSTRAINT [DF__Albaranes__fkalb__2BE97B0D]  DEFAULT ((1)),
	[id] [int] NOT NULL,
	[fkarticulos] [varchar](15) NULL,
	[descripcion] [varchar](120) NULL,
	[lote] [varchar](12) NULL,
	[tabla] [int] NULL,
	[cantidad] [float] NULL,
	[cantidadpedida] [float] NULL,
	[largo] [float] NULL,
	[ancho] [float] NULL,
	[grueso] [float] NULL,
	[fkunidades] [varchar](2) NULL,
	[metros] [float] NULL,
	[precio] [float] NULL,
	[porcentajedescuento] [float] NULL,
	[importedescuento] [float] NULL,
	[fktiposiva] [varchar](3) NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[cuotarecargoequivalencia] [float] NULL,
	[importe] [float] NULL,
	[importenetolinea] [float] NULL,
	[notas] [text] NULL,
	[documentoorigen] [varchar](15) NULL,
	[documentodestino] [varchar](15) NULL,
	[canal] [varchar](3) NULL,
	[precioanterior] [float] NULL,
	[revision] [varchar](1) NULL,
	[decimalesmonedas] [int] NULL,
	[decimalesmedidas] [int] NULL,
	[bundle] [varchar](2) NULL,
	[tblnum] [int] NULL,
	[contenedor] [varchar](12) NULL,
	[sello] [varchar](10) NULL,
	[caja] [int] NULL,
	[pesoneto] [float] NULL,
	[pesobruto] [float] NULL,
	[seccion] [varchar](4) NULL,
	[costeadicionalmaterial] [float] NULL,
	[costeadicionalportes] [float] NULL,
	[costeadicionalotro] [float] NULL,
	[costeacicionalvariable] [float] NULL,
	[orden] [int] NULL,
	[fkpedidos] [int] NULL,
	[fkpedidosid] [int] NULL,
	[fkpedidosreferencia] [varchar](30) NULL,
 CONSTRAINT [PK_AlbaranesLin_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkalbaranes] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlbaranesTotales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AlbaranesTotales](
	[empresa] [varchar](4) NOT NULL,
	[fkalbaranes] [int] NOT NULL CONSTRAINT [DF__Albaranes__fkalb__2CDD9F46]  DEFAULT ((1)),
	[fktiposiva] [varchar](3) NOT NULL,
	[brutototal] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[importerecargoequivalencia] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[basetotal] [float] NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[subtotal] [float] NULL,
	[decimalesmonedas] [int] NULL,
 CONSTRAINT [PK_AlbaranesTotales_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkalbaranes] ASC,
	[fktiposiva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AppPermisos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AppPermisos](
	[id] [uniqueidentifier] NOT NULL,
	[descripcion] [varchar](100) NULL,
	[preferencias] [text] NULL,
 CONSTRAINT [PK_AppPermisos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AppPermisosRoles]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppPermisosRoles](
	[fkRoles] [uniqueidentifier] NOT NULL,
	[fkAppPermisos] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AppPermisosRoles] PRIMARY KEY CLUSTERED 
(
	[fkRoles] ASC,
	[fkAppPermisos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AppPermisosUsuarios]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppPermisosUsuarios](
	[fkUsuarios] [uniqueidentifier] NOT NULL,
	[fkAppPermisos] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AppPermisosUsuarios] PRIMARY KEY CLUSTERED 
(
	[fkUsuarios] ASC,
	[fkAppPermisos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Articulos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Articulos](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](15) NOT NULL,
	[descripcion] [varchar](120) NULL,
	[descripcion2] [varchar](120) NULL,
	[descripcionabreviada] [varchar](120) NULL,
	[coste] [float] NULL,
	[preciominimoventa] [float] NULL,
	[preciomateriaprima] [float] NULL,
	[porcentajemerma] [float] NULL,
	[costemateriaprima] [float] NULL,
	[costeelaboracionmateriaprima] [float] NULL,
	[costeportes] [float] NULL,
	[otroscostes] [float] NULL,
	[costefabricacion] [float] NULL,
	[costeindirecto] [float] NULL,
	[largo] [float] NULL,
	[ancho] [float] NULL,
	[grueso] [float] NULL,
	[fkgruposiva] [varchar](4) NULL,
	[fkguiascontables] [varchar](12) NULL,
	[editarlargo] [bit] NULL,
	[editarancho] [bit] NULL,
	[editargrueso] [bit] NULL,
	[fkgruposmateriales] [varchar](3) NULL,
	[partidaarancelaria] [varchar](10) NULL,
	[kilosud] [float] NULL,
	[medidalibre] [bit] NULL,
	[labor] [bit] NULL,
	[fklabores] [varchar](3) NULL,
	[excluircomisiones] [bit] NULL,
	[exentoretencion] [bit] NULL,
	[gestionarcaducidad] [bit] NULL,
	[tiempostandardfabricacion] [float] NULL,
	[gestionstock] [bit] NULL,
	[tipogestionlotes] [int] NULL,
	[stocknegativoautorizado] [bit] NULL,
	[existenciasminimasmetros] [float] NULL,
	[existenciasmaximasmetros] [float] NULL,
	[existenciasminimasunidades] [float] NULL,
	[existenciasmaximasunidades] [float] NULL,
	[web] [bit] NULL,
	[rendimientom2m3] [float] NULL,
	[articulonegocio] [bit] NULL,
 CONSTRAINT [PK_Articulos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Aseguradoras]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Aseguradoras](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](10) NOT NULL,
	[numeropoliza] [varchar](25) NULL,
	[fechainicio] [datetime] NULL,
	[fechafin] [datetime] NULL,
	[diasimpago] [int] NULL,
	[diasimpagovencimientoprorrogado] [int] NULL,
	[numeroprorrogas] [int] NULL,
	[primaanual] [float] NULL,
	[numerorecibos] [int] NULL,
	[porcentajecoberturariesgo] [float] NULL,
 CONSTRAINT [PK_Aseguradoras] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Bancos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Bancos](
	[id] [varchar](4) NOT NULL,
	[nombre] [varchar](50) NULL,
	[bic] [varchar](50) NULL,
 CONSTRAINT [PK_Bancos] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BancosMandatos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BancosMandatos](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[id] [varchar](3) NOT NULL,
	[descripcion] [varchar](100) NULL,
	[fkpaises] [varchar](3) NULL,
	[iban] [varchar](34) NULL,
	[bic] [varchar](15) NULL,
	[sufijoacreedor] [varchar](3) NULL,
	[contratoconfirmig] [varchar](12) NULL,
	[contadorconfirming] [varchar](10) NULL,
	[direccion] [varchar](100) NULL,
	[cpostal] [varchar](10) NULL,
	[ciudad] [varchar](50) NULL,
	[fkprovincias] [varchar](2) NULL,
	[telefonobanco] [varchar](20) NULL,
	[personacontacto] [varchar](50) NULL,
	[idmandato] [varchar](35) NULL,
	[idacreedor] [varchar](35) NULL,
	[tiposecuenciasepa] [int] NULL,
	[tipoadeudo] [int] NULL,
	[importemandato] [float] NULL,
	[recibosmandato] [int] NULL,
	[importelimiterecibo] [float] NULL,
	[fechafirma] [datetime] NULL,
	[fechaexpiracion] [datetime] NULL,
	[fechaultimaremesa] [datetime] NULL,
	[importeremesados] [float] NULL,
	[recibosremesados] [int] NULL,
	[devolvera] [varchar](60) NULL,
	[notas] [text] NULL,
	[defecto] [bit] NULL,
	[finalizado] [bit] NULL,
	[esquema] [int] NULL,
	[bloqueada] [bit] NULL,
	[fkMotivosbloqueo] [varchar](3) NULL,
	[fechamodificacionbloqueo] [datetime] NULL,
	[fkUsuariobloqueo] [uniqueidentifier] NULL,
	[riesgonacional] [varchar](50) NULL,
	[riesgoextranjero] [varchar](50) NULL,
 CONSTRAINT [PK_BancosMandatos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Caracteristicas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Caracteristicas](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](2) NOT NULL,
 CONSTRAINT [PK_Caracteristicas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CaracteristicasLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CaracteristicasLin](
	[empresa] [varchar](4) NOT NULL,
	[fkcaracteristicas] [varchar](2) NOT NULL,
	[id] [varchar](2) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[descripcion2] [varchar](40) NULL,
	[descripcionabreviada] [varchar](20) NULL,
 CONSTRAINT [PK_CaracteristicasLin] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcaracteristicas] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Carpetas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Carpetas](
	[empresa] [varchar](4) NOT NULL,
	[id] [uniqueidentifier] NOT NULL,
	[nombre] [varchar](100) NULL,
	[fkcarpetas] [uniqueidentifier] NULL,
	[ruta] [varchar](max) NULL,
 CONSTRAINT [PK_Carpetas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Clientes](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fkidiomas] [varchar](3) NOT NULL,
	[fkfamiliacliente] [varchar](3) NULL,
	[fkzonacliente] [varchar](3) NULL,
	[fktipoempresa] [varchar](3) NULL,
	[fkunidadnegocio] [varchar](3) NULL,
	[fkincoterm] [varchar](3) NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[fkmonedas] [int] NOT NULL,
	[fkregimeniva] [varchar](5) NOT NULL,
	[fkgruposiva] [varchar](4) NULL,
	[criterioiva] [int] NOT NULL,
	[fktiposretencion] [varchar](4) NULL,
	[fktransportistahabitual] [varchar](15) NULL,
	[tipoportes] [int] NULL,
	[cuentatesoreria] [varchar](15) NULL,
	[fkformaspago] [int] NOT NULL,
	[descuentoprontopago] [float] NULL,
	[descuentocomercial] [float] NULL,
	[diafijopago1] [int] NULL,
	[diafijopago2] [int] NULL,
	[periodonopagodesde] [varchar](5) NULL,
	[periodonopagohasta] [varchar](5) NULL,
	[notas] [text] NULL,
	[numerocopiasfactura] [int] NULL,
	[fkcuentasagente] [varchar](15) NULL,
	[fkcuentascomercial] [varchar](15) NULL,
	[perteneceagrupo] [varchar](50) NULL,
	[tarifa] [int] NULL,
	[fkcuentasaseguradoras] [varchar](15) NULL,
	[suplemento] [varchar](10) NULL,
	[porcentajeriesgocomercial] [float] NULL,
	[porcentajeriesgopolitico] [float] NULL,
	[riesgoconcedidoempresa] [int] NULL,
	[riesgosolicitado] [int] NULL,
	[riesgoaseguradora] [int] NULL,
	[fechaclasificacion] [datetime] NULL,
	[fechaultimasolicitud] [datetime] NULL,
	[diascondecidos] [int] NULL,
	[fktarifas] [varchar](12) NULL,
	[fkcriteriosagrupacion] [varchar](4) NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Comerciales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Comerciales](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fktipoirpf] [varchar](4) NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fkformapago] [int] NULL,
	[porcentajecomision] [float] NULL,
	[comisionporm2] [float] NULL,
	[comisionporm3] [float] NULL,
	[porcentajeincrementosobreptb] [float] NULL,
	[primaincrementosobreptb] [float] NULL,
	[porcentajedecrementosobreptb] [float] NULL,
	[primadecrementosobreptb] [float] NULL,
 CONSTRAINT [PK_Comerciales] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Configuracion]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuracion](
	[id] [int] NOT NULL,
	[xml] [xml] NULL,
 CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contactos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Contactos](
	[empresa] [varchar](4) NOT NULL,
	[tipotercero] [int] NOT NULL,
	[fkentidad] [varchar](15) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
	[fktipocontacto] [varchar](3) NULL,
	[fkcargoempresa] [varchar](3) NULL,
	[fkidioma] [varchar](3) NULL,
	[telefono] [varchar](15) NULL,
	[telefonomovil] [varchar](15) NULL,
	[fax] [varchar](15) NULL,
	[email] [varchar](200) NULL,
	[nifcif] [varchar](15) NULL,
	[observaciones] [text] NULL,
	[fktipodireccion_direccion] [int] NULL,
	[fkentidad_direccion] [varchar](15) NULL,
	[fkid_direccion] [int] NULL,
	[fktipoidentificacion_nif] [varchar](3) NULL,
 CONSTRAINT [PK_Contactos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[tipotercero] ASC,
	[fkentidad] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Contadores]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Contadores](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](12) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[tipoinicio] [int] NULL,
	[primerdocumento] [int] NULL,
	[tipocontador] [int] NULL,
 CONSTRAINT [PK_Contador] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ContadoresLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContadoresLin](
	[empresa] [varchar](4) NOT NULL,
	[fkcontadores] [varchar](12) NOT NULL,
	[id] [int] NOT NULL,
	[tiposegmento] [int] NULL,
	[longitud] [int] NULL,
	[valor] [varchar](20) NULL,
 CONSTRAINT [PK_ContadoresLin] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcontadores] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Criteriosagrupacion]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Criteriosagrupacion](
	[id] [varchar](4) NOT NULL,
	[nombre] [varchar](150) NULL,
	[ordenaralbaranes] [bit] NULL,
 CONSTRAINT [PK_Criteriosagrupacion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CriteriosagrupacionLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CriteriosagrupacionLin](
	[fkcriteriosagrupacion] [varchar](4) NOT NULL,
	[id] [int] NOT NULL,
	[campo] [int] NOT NULL,
	[orden] [int] NULL,
 CONSTRAINT [PK_CriteriosagrupacionLin] PRIMARY KEY CLUSTERED 
(
	[fkcriteriosagrupacion] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cuentas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cuentas](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](15) NOT NULL,
	[descripcion] [varchar](200) NULL,
	[descripcion2] [varchar](200) NULL,
	[nivel] [int] NULL,
	[nif] [varchar](15) NULL,
	[fkPais] [varchar](3) NULL,
	[bloqueada] [bit] NULL,
	[fkMotivosbloqueo] [varchar](3) NULL,
	[tipocuenta] [int] NULL,
	[fechaalta] [datetime] NULL,
	[fechamodificacion] [datetime] NULL,
	[fkUsuarios] [uniqueidentifier] NULL,
	[fechamodificacionbloqueo] [datetime] NULL,
	[fkUsuariobloqueo] [uniqueidentifier] NULL,
	[contrapartida] [varchar](10) NULL,
	[fktipoidentificacion_nif] [varchar](3) NULL,
	[categoria] [int] NULL,
 CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cuentastesoreria]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cuentastesoreria](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
 CONSTRAINT [PK_Cuentastesoreria] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Direcciones]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Direcciones](
	[empresa] [varchar](4) NOT NULL,
	[tipotercero] [int] NOT NULL,
	[fkentidad] [varchar](15) NOT NULL,
	[id] [int] NOT NULL,
	[defecto] [bit] NULL,
	[descripcion] [varchar](100) NULL,
	[fktipovia] [varchar](3) NULL,
	[direccion] [text] NULL,
	[poblacion] [varchar](100) NULL,
	[cp] [varchar](10) NULL,
	[fkpais] [varchar](3) NULL,
	[fkprovincia] [varchar](2) NULL,
	[personacontacto] [varchar](30) NULL,
	[telefono] [varchar](15) NULL,
	[telefonomovil] [varchar](15) NULL,
	[fax] [varchar](15) NULL,
	[email] [varchar](200) NULL,
	[web] [text] NULL,
	[notas] [text] NULL,
	[fktipodireccion] [varchar](3) NULL,
 CONSTRAINT [PK_Direcciones] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[tipotercero] ASC,
	[fkentidad] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentosUsuario]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentosUsuario](
	[fkusuario] [uniqueidentifier] NOT NULL,
	[tipo] [int] NOT NULL,
	[nombre] [varchar](100) NOT NULL,
	[tipoprivacidad] [int] NOT NULL,
	[tiporeport] [int] NOT NULL,
	[datos] [varbinary](max) NULL,
 CONSTRAINT [PK_DocumentosUsuario_1] PRIMARY KEY CLUSTERED 
(
	[fkusuario] ASC,
	[tipo] ASC,
	[nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ejercicios]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ejercicios](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](120) NULL,
	[descripcioncorta] [varchar](10) NULL,
	[desde] [datetime] NULL,
	[hasta] [datetime] NULL,
	[estado] [int] NULL,
	[contabilidadcerradahasta] [datetime] NULL,
	[registroivacerradohasta] [datetime] NULL,
	[criterioiva] [int] NULL,
	[fkejercicios] [int] NULL,
 CONSTRAINT [PK_Ejercicios] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Empresas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Empresas](
	[id] [varchar](4) NOT NULL,
	[nombre] [varchar](50) NULL,
	[nif] [varchar](15) NULL,
	[fktipoidentificacion_nif] [varchar](3) NULL,
	[razonsocial] [varchar](200) NULL,
	[fkPais] [varchar](3) NULL,
	[Fkplangeneralcontable] [uniqueidentifier] NULL,
	[administrador] [varchar](100) NULL,
	[nifcifadministrador] [varchar](15) NULL,
	[fktipoidentificacion_nifcifadministrador] [varchar](3) NULL,
	[actividadprincipal] [varchar](100) NULL,
	[cnae] [varchar](10) NULL,
	[nivel] [int] NULL,
	[fkMonedabase] [int] NULL,
	[fkMonedaadicional] [int] NULL,
	[digitoscuentas] [int] NULL,
	[nivelcuentas] [int] NULL,
	[cuentasanuales] [varchar](100) NULL,
	[cuentasperdidas] [varchar](100) NULL,
	[criterioiva] [int] NULL,
	[liquidacioniva] [int] NULL,
	[tipoempresa] [varchar](3) NULL,
	[datosregistrales] [text] NULL,
	[fktarifasventas] [varchar](12) NULL,
	[fktarifascompras] [varchar](12) NULL,
	[fkregimeniva] [varchar](5) NULL,
 CONSTRAINT [PK_Empresas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Estados]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Estados](
	[documento] [int] NOT NULL,
	[id] [varchar](3) NOT NULL,
	[descripcion] [varchar](25) NULL,
	[imputariesgo] [bit] NULL,
	[tipoestado] [int] NULL,
	[notas] [text] NULL,
	[tipomovimiento] [int] NULL,
 CONSTRAINT [PK_Estados] PRIMARY KEY CLUSTERED 
(
	[documento] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Facturas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Facturas](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fkejercicio] [int] NOT NULL CONSTRAINT [DF__Facturas__fkejer__31A25463]  DEFAULT ((1)),
	[fkseries] [varchar](3) NOT NULL,
	[identificadorsegmento] [varchar](12) NOT NULL,
	[referencia] [varchar](30) NULL,
	[fechadocumento] [datetime] NULL,
	[fkalmacen] [int] NULL,
	[fkclientes] [varchar](15) NULL,
	[nombrecliente] [varchar](200) NULL,
	[clientedireccion] [varchar](100) NULL,
	[clientepoblacion] [varchar](100) NULL,
	[clientecp] [varchar](10) NULL,
	[clientepais] [varchar](50) NULL,
	[clienteprovincia] [varchar](50) NULL,
	[clientetelefono] [varchar](50) NULL,
	[clientefax] [varchar](15) NULL,
	[clienteemail] [varchar](200) NULL,
	[clientenif] [varchar](15) NULL,
	[fkformaspago] [int] NULL,
	[fkmonedas] [int] NULL,
	[cambioadicional] [float] NULL,
	[cobrado] [bit] NULL,
	[facturarectificativa] [bit] NULL,
	[fkfacturarectificada] [varchar](30) NULL,
	[fkmotivorectificacion] [varchar](3) NULL,
	[importebruto] [float] NULL,
	[importebaseimponible] [float] NULL,
	[importeportes] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[importetotaldoc] [float] NULL,
	[importetotalmonedabase] [float] NULL,
	[notas] [text] NULL,
	[fkestados] [varchar](10) NULL,
	[fkobras] [varchar](15) NULL,
	[incoterm] [varchar](3) NULL,
	[descripcionincoterm] [varchar](30) NULL,
	[peso] [int] NULL,
	[confianza] [int] NULL,
	[costemateriales] [float] NULL,
	[tiempooficinatecnica] [float] NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fktransportista] [varchar](15) NULL,
	[tipocambio] [float] NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[unidadnegocio] [varchar](3) NULL,
	[referenciadocumento] [varchar](30) NULL,
	[fkbancosmandatos] [varchar](3) NULL,
	[cartacredito] [varchar](25) NULL,
	[vencimientocartacredito] [datetime] NULL,
	[contenedores] [int] NULL,
	[fkcuentastesoreria] [varchar](15) NULL,
	[numerodocumentoproveedor] [varchar](30) NULL,
	[fechadocumentoproveedor] [datetime] NULL,
	[fkclientesreserva] [varchar](15) NULL,
	[tipoalbaran] [int] NOT NULL,
	[fkmotivosdevolucion] [varchar](3) NULL,
	[nombretransportista] [varchar](40) NULL,
	[conductor] [varchar](20) NULL,
	[matricula] [varchar](12) NULL,
	[bultos] [int] NULL,
	[pesoneto] [float] NULL,
	[pesobruto] [float] NULL,
	[volumen] [float] NULL,
	[envio] [text] NULL,
	[fkoperarios] [varchar](15) NULL,
	[fkoperadortransporte] [varchar](15) NULL,
	[fkzonas] [varchar](3) NULL,
	[fkdireccionfacturacion] [int] NULL,
	[brutocomision] [float] NULL,
	[comisiondescontardescuentocomercial] [bit] NULL,
	[comsiondescontardescuentoprontopago] [bit] NULL,
	[cuotadescuentocomercialcomision] [float] NULL,
	[cuotadescuentoprontopagocomision] [float] NULL,
	[basecomision] [float] NULL,
	[comisiondescontarportesincluidosprecio] [bit] NULL,
	[cuotadescuentoportesincluidospreciocomision] [float] NULL,
	[comisiondescontarrecargofinancieroformapago] [bit] NULL,
	[cuotadescuentorecargofinancieroformapagocomision] [float] NULL,
	[netobasecomision] [float] NULL,
	[importecomision] [float] NULL,
	[fksituacioncomision] [varchar](3) NULL,
	[fkaseguradoras] [varchar](15) NULL,
	[suplemento] [varchar](20) NULL,
	[fktiposretenciones] [varchar](4) NULL,
	[porcentajeretencion] [float] NULL,
	[fkagentes] [varchar](15) NULL,
	[fkcomerciales] [varchar](15) NULL,
	[comisionagente] [float] NULL,
	[comisioncomercial] [float] NULL,
	[idmandato] [varchar](35) NULL,
	[fkusuarioalta] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Facturas__fkusua__2DD1C37F]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechaalta] [datetime] NOT NULL CONSTRAINT [DF__Facturas__fechaa__2EC5E7B8]  DEFAULT (getdate()),
	[fkusuariomodificacion] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Facturas__fkusua__2FBA0BF1]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechamodificacion] [datetime] NOT NULL CONSTRAINT [DF__Facturas__fecham__30AE302A]  DEFAULT (getdate()),
	[fkcarpetas] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Facturas_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FacturasLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FacturasLin](
	[empresa] [varchar](4) NOT NULL,
	[fkfacturas] [int] NOT NULL CONSTRAINT [DF__FacturasL__fkfac__3296789C]  DEFAULT ((1)),
	[id] [int] NOT NULL,
	[fkarticulos] [varchar](15) NULL,
	[descripcion] [varchar](120) NULL,
	[lote] [varchar](12) NULL,
	[tabla] [int] NULL,
	[cantidad] [float] NULL,
	[cantidadpedida] [float] NULL,
	[largo] [float] NULL,
	[ancho] [float] NULL,
	[grueso] [float] NULL,
	[fkunidades] [varchar](2) NULL,
	[metros] [float] NULL,
	[precio] [float] NULL,
	[porcentajedescuento] [float] NULL,
	[importedescuento] [float] NULL,
	[fktiposiva] [varchar](3) NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[cuotarecargoequivalencia] [float] NULL,
	[importe] [float] NULL,
	[importenetolinea] [float] NULL,
	[notas] [text] NULL,
	[documentoorigen] [varchar](15) NULL,
	[documentodestino] [varchar](15) NULL,
	[canal] [varchar](3) NULL,
	[precioanterior] [float] NULL,
	[revision] [varchar](1) NULL,
	[decimalesmonedas] [int] NULL,
	[decimalesmedidas] [int] NULL,
	[bundle] [varchar](2) NULL,
	[tblnum] [int] NULL,
	[contenedor] [varchar](12) NULL,
	[sello] [varchar](10) NULL,
	[caja] [int] NULL,
	[fkalbaranesfecha] [datetime] NULL,
	[fkalbaranesreferencia] [varchar](30) NULL,
	[fkalbaranesfkcriteriosagrupacion] [varchar](4) NULL,
	[pesoneto] [float] NULL,
	[pesobruto] [float] NULL,
	[seccion] [varchar](4) NULL,
	[costeadicionalmaterial] [float] NULL,
	[costeadicionalportes] [float] NULL,
	[costeadicionalotro] [float] NULL,
	[costeacicionalvariable] [float] NULL,
	[orden] [int] NULL,
	[porcentajecomision] [float] NULL,
	[fkalbaranes] [int] NULL,
	[fkalbaranesid] [int] NULL,
 CONSTRAINT [PK_FacturasLin_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkfacturas] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FacturasTotales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FacturasTotales](
	[empresa] [varchar](4) NOT NULL,
	[fkfacturas] [int] NOT NULL CONSTRAINT [DF__FacturasT__fkfac__338A9CD5]  DEFAULT ((1)),
	[fktiposiva] [varchar](3) NOT NULL,
	[brutototal] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[importerecargoequivalencia] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[basetotal] [float] NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[subtotal] [float] NULL,
	[decimalesmonedas] [int] NULL,
 CONSTRAINT [PK_FacturasTotales_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkfacturas] ASC,
	[fktiposiva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FacturasVencimientos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FacturasVencimientos](
	[empresa] [varchar](4) NOT NULL,
	[fkfacturas] [int] NOT NULL CONSTRAINT [DF__FacturasV__fkfac__347EC10E]  DEFAULT ((1)),
	[id] [int] NOT NULL,
	[diasvencimiento] [int] NULL,
	[fechavencimiento] [datetime] NULL,
	[importevencimiento] [float] NULL,
 CONSTRAINT [PK_FacturasVencimientos_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkfacturas] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Familiasproductos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Familiasproductos](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](2) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[descripcion2] [varchar](40) NULL,
	[descripcionabreviada] [varchar](20) NULL,
	[fkcontador] [varchar](10) NULL,
	[fkguiascontables] [varchar](12) NULL,
	[tipofamilia] [int] NULL,
	[gestionstock] [bit] NULL,
	[articulonegocio] [bit] NULL,
	[fkunidadesmedida] [varchar](2) NULL,
	[editarlargo] [bit] NULL,
	[editarancho] [bit] NULL,
	[editargrueso] [bit] NULL,
	[validarmaterial] [bit] NULL,
	[validarcaracteristica] [bit] NULL,
	[validargrosor] [bit] NULL,
	[validaracabado] [bit] NULL,
	[descripcion1generada] [varchar](12) NULL,
	[descripcion2generada] [varchar](12) NULL,
	[descripcionabreviadagenerada] [varchar](12) NULL,
	[fkgruposiva] [varchar](4) NULL,
	[tipogestionlotes] [int] NULL,
	[stocknegativoautorizado] [bit] NULL,
	[existenciasminimasmetros] [float] NULL,
	[existenciasmaximasmetros] [float] NULL,
	[existenciasminimasunidades] [float] NULL,
	[existenciasmaximasunidades] [float] NULL,
	[web] [bit] NULL,
 CONSTRAINT [PK_Familiasproductos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Ficheros]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ficheros](
	[empresa] [varchar](4) NOT NULL,
	[id] [uniqueidentifier] NOT NULL,
	[nombre] [varchar](200) NULL,
	[descripcion] [text] NULL,
	[ruta] [varchar](max) NULL,
	[tipo] [varchar](10) NULL,
	[fkcarpetas] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Ficheros] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FormasPago]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FormasPago](
	[id] [int] NOT NULL,
	[nombre] [varchar](100) NULL,
	[nombre2] [varchar](100) NULL,
	[fkModosPago] [varchar](3) NULL,
	[imprimirvencimientos] [bit] NULL,
	[recargofinanciero] [float] NULL,
	[efectivo] [bit] NULL,
	[remesable] [bit] NULL,
	[mandato] [bit] NULL,
	[excluirfestivos] [bit] NULL,
	[bloqueada] [bit] NULL,
	[fkMotivosbloqueo] [varchar](3) NULL,
	[fechamodificacionbloqueo] [datetime] NULL,
	[fkUsuariobloqueo] [uniqueidentifier] NULL,
	[fkgruposformaspago] [varchar](3) NULL,
 CONSTRAINT [PK_FormasPago] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FormasPagoLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormasPagoLin](
	[fkFormasPago] [int] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[diasvencimiento] [smallint] NULL,
	[porcentajerecargo] [float] NULL,
 CONSTRAINT [PK_FormasPagoLin] PRIMARY KEY CLUSTERED 
(
	[fkFormasPago] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Grosores]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Grosores](
	[id] [varchar](2) NOT NULL,
	[grosor] [float] NULL,
	[descripcion] [varchar](40) NULL,
	[descripcion2] [varchar](40) NULL,
	[descripcionabreviada] [varchar](20) NULL,
	[coficientecortabloques] [float] NULL,
	[coeficientetelares] [float] NULL,
 CONSTRAINT [PK_Grosores] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GruposIva]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GruposIva](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](4) NOT NULL,
	[descripcion] [varchar](50) NULL,
 CONSTRAINT [PK_GruposIva_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GruposIvaLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GruposIvaLin](
	[empresa] [varchar](4) NOT NULL,
	[fkgruposiva] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[desde] [datetime] NOT NULL,
	[fktiposivasinrecargo] [varchar](3) NULL,
	[fktiposivaconrecargo] [varchar](3) NULL,
	[fktiposivaexentoiva] [varchar](3) NULL,
 CONSTRAINT [PK_gruposiva] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkgruposiva] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Guiascontables]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Guiascontables](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](12) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[fkcuentascompras] [varchar](15) NULL,
	[fkcuentasventas] [varchar](15) NULL,
	[fkcuentasdevolucioncompras] [varchar](15) NULL,
	[fkcuentasdevolucionventas] [varchar](15) NULL,
 CONSTRAINT [PK_Guiascontables] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GuiascontablesLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GuiascontablesLin](
	[empresa] [varchar](4) NOT NULL,
	[fkguiascontables] [varchar](12) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fkcuentascompras] [varchar](15) NULL,
	[fkcuentasventas] [varchar](15) NULL,
	[fkcuentasdevolucioncompras] [varchar](15) NULL,
	[fkcuentasdevolucionventas] [varchar](15) NULL,
 CONSTRAINT [PK_GuiascontablesLin] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkguiascontables] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Materiales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Materiales](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](3) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[descripcion2] [varchar](40) NULL,
	[descripcionabreviada] [varchar](40) NULL,
	[fkfamiliamateriales] [varchar](3) NULL,
	[fkgruposmateriales] [varchar](3) NULL,
	[dureza] [varchar](1) NULL,
 CONSTRAINT [PK_Materiales] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MaterialesLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MaterialesLin](
	[empresa] [varchar](4) NOT NULL,
	[fkmateriales] [varchar](3) NOT NULL,
	[codigovariedad] [varchar](20) NOT NULL,
	[descripcion] [varchar](30) NULL,
	[descripcion2] [varchar](30) NULL,
 CONSTRAINT [PK_MaterialesLin_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkmateriales] ASC,
	[codigovariedad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Monedas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Monedas](
	[id] [int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
	[abreviatura] [varchar](3) NOT NULL,
	[decimales] [int] NULL,
	[cambiomonedabase] [float] NULL,
	[cambiomonedaadicional] [float] NULL,
	[fechamodificacion] [datetime] NULL,
	[fkUsuarios] [uniqueidentifier] NULL,
	[fkUsuariosnombre] [varchar](20) NULL,
	[activado] [bit] NULL,
 CONSTRAINT [PK_Monedas] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MonedasLog]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MonedasLog](
	[fkMonedas] [int] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cambiomonedabase] [float] NULL,
	[cambiomonedaadicional] [float] NULL,
	[fechamodificacion] [datetime] NULL,
	[fkUsuarios] [uniqueidentifier] NULL,
	[fkUsuariosnombre] [varchar](20) NULL,
 CONSTRAINT [PK_MonedasLog] PRIMARY KEY CLUSTERED 
(
	[fkMonedas] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Obras]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Obras](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] NOT NULL,
	[fkclientes] [varchar](15) NULL,
	[nombreobra] [varchar](50) NULL,
	[fkdirecciones] [int] NULL,
	[fktiposobra] [varchar](3) NULL,
	[fkagentes] [varchar](15) NULL,
	[fkcomerciales] [varchar](15) NULL,
	[fkregimeniva] [varchar](5) NULL,
	[retencion] [float] NULL,
	[fechainicio] [datetime] NULL,
	[fechafin] [datetime] NULL,
	[fecharevision] [datetime] NULL,
	[certificado] [varchar](10) NULL,
	[notas] [text] NULL,
	[email] [varchar](200) NULL,
	[finalizada] [bit] NULL,
 CONSTRAINT [PK_Obras] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Operarios]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Operarios](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fechanacimiento] [datetime] NULL,
	[fkestadocivil] [varchar](3) NULL,
	[numerohijos] [int] NULL,
	[fechaingreso] [datetime] NULL,
	[fkcargo] [varchar](3) NULL,
	[numeroseguridadsocial] [varchar](20) NULL,
	[fkcontratoactual] [varchar](3) NULL,
	[fechaalta] [datetime] NULL,
	[vencimientocontrato] [datetime] NULL,
	[ultimafechabaja] [datetime] NULL,
	[ultimafechaalta] [datetime] NULL,
	[tallacamisa] [varchar](4) NULL,
	[tallapantalon] [varchar](4) NULL,
	[tallacalzado] [varchar](4) NULL,
	[notas] [text] NULL,
	[fkcuentatesoreria] [varchar](15) NULL,
 CONSTRAINT [PK_Operarios] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Pedidos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Pedidos](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fkejercicio] [int] NOT NULL CONSTRAINT [DF__Pedidos__fkejerc__24485945]  DEFAULT ((1)),
	[fkseries] [varchar](3) NOT NULL,
	[identificadorsegmento] [varchar](12) NOT NULL,
	[referencia] [varchar](30) NULL,
	[fechadocumento] [datetime] NULL,
	[fechavalidez] [datetime] NULL,
	[fechaentrega] [datetime] NULL,
	[fecharevision] [datetime] NULL,
	[fkalmacen] [int] NULL,
	[fkclientes] [varchar](15) NULL,
	[nombrecliente] [varchar](200) NULL,
	[clientedireccion] [varchar](100) NULL,
	[clientepoblacion] [varchar](100) NULL,
	[clientecp] [varchar](10) NULL,
	[clientepais] [varchar](50) NULL,
	[clienteprovincia] [varchar](50) NULL,
	[clientetelefono] [varchar](50) NULL,
	[clientefax] [varchar](15) NULL,
	[clienteemail] [varchar](200) NULL,
	[clientenif] [varchar](15) NULL,
	[fkformaspago] [int] NULL,
	[fkagentes] [varchar](15) NULL,
	[fkcomerciales] [varchar](15) NULL,
	[comisionagente] [float] NULL,
	[comisioncomercial] [float] NULL,
	[fkmonedas] [int] NULL,
	[cambioadicional] [float] NULL,
	[importebruto] [float] NULL,
	[importebaseimponible] [float] NULL,
	[importeportes] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[importetotaldoc] [float] NULL,
	[importetotalmonedabase] [float] NULL,
	[notas] [text] NULL,
	[fkestados] [varchar](10) NULL,
	[fkobras] [varchar](15) NULL,
	[incoterm] [varchar](3) NULL,
	[descripcionincoterm] [varchar](30) NULL,
	[peso] [int] NULL,
	[confianza] [int] NULL,
	[costemateriales] [float] NULL,
	[tiempooficinatecnica] [float] NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fktransportista] [varchar](15) NULL,
	[tipocambio] [float] NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[unidadnegocio] [varchar](3) NULL,
	[referenciadocumento] [varchar](30) NULL,
	[fkbancosmandatos] [varchar](3) NULL,
	[cartacredito] [varchar](25) NULL,
	[vencimientocartacredito] [datetime] NULL,
	[contenedores] [int] NULL,
	[fkcuentastesoreria] [varchar](15) NULL,
	[fkusuarioalta] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Pedidos__fkusuar__2077C861]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechaalta] [datetime] NOT NULL CONSTRAINT [DF__Pedidos__fechaal__216BEC9A]  DEFAULT (getdate()),
	[fkusuariomodificacion] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Pedidos__fkusuar__226010D3]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechamodificacion] [datetime] NOT NULL CONSTRAINT [DF__Pedidos__fechamo__2354350C]  DEFAULT (getdate()),
	[fkcarpetas] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Pedidos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PedidosLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PedidosLin](
	[empresa] [varchar](4) NOT NULL,
	[fkpedidos] [int] NOT NULL CONSTRAINT [DF__PedidosLi__fkped__253C7D7E]  DEFAULT ((1)),
	[id] [int] NOT NULL,
	[fkarticulos] [varchar](15) NULL,
	[descripcion] [varchar](120) NULL,
	[lote] [varchar](12) NULL,
	[tabla] [int] NULL,
	[cantidad] [float] NULL,
	[cantidadpedida] [float] NULL,
	[largo] [float] NULL,
	[ancho] [float] NULL,
	[grueso] [float] NULL,
	[fkunidades] [varchar](2) NULL,
	[metros] [float] NULL,
	[precio] [float] NULL,
	[porcentajedescuento] [float] NULL,
	[importedescuento] [float] NULL,
	[fktiposiva] [varchar](3) NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[cuotarecargoequivalencia] [float] NULL,
	[importe] [float] NULL,
	[notas] [text] NULL,
	[documentoorigen] [varchar](15) NULL,
	[documentodestino] [varchar](15) NULL,
	[canal] [varchar](3) NULL,
	[precioanterior] [float] NULL,
	[revision] [varchar](1) NULL,
	[decimalesmonedas] [int] NULL,
	[decimalesmedidas] [int] NULL,
	[labor1l1] [int] NULL,
	[labor2l1] [varchar](3) NULL,
	[labor3l1] [varchar](3) NULL,
	[labor4l1] [varchar](3) NULL,
	[labor1l2] [int] NULL,
	[labor2l2] [varchar](3) NULL,
	[labor3l2] [varchar](3) NULL,
	[labor4l2] [varchar](3) NULL,
	[labor1l3] [int] NULL,
	[labor2l3] [varchar](3) NULL,
	[labor3l3] [varchar](3) NULL,
	[labor4l3] [varchar](3) NULL,
	[labor1l4] [int] NULL,
	[labor2l4] [varchar](3) NULL,
	[labor3l4] [varchar](3) NULL,
	[labor4l4] [varchar](3) NULL,
	[bundle] [varchar](2) NULL,
	[tblnum] [int] NULL,
	[caja] [int] NULL,
	[fkpresupuestos] [int] NULL,
	[fkpresupuestosid] [int] NULL,
	[fkpresupuestosreferencia] [varchar](30) NULL,
 CONSTRAINT [PK_PedidosLin_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkpedidos] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PedidosTotales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PedidosTotales](
	[empresa] [varchar](4) NOT NULL,
	[fkpedidos] [int] NOT NULL CONSTRAINT [DF__PedidosTo__fkped__2630A1B7]  DEFAULT ((1)),
	[fktiposiva] [varchar](3) NOT NULL,
	[brutototal] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[importerecargoequivalencia] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[basetotal] [float] NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[subtotal] [float] NULL,
	[decimalesmonedas] [int] NULL,
 CONSTRAINT [PK_PedidosTotales_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkpedidos] ASC,
	[fktiposiva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Planesgenerales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Planesgenerales](
	[id] [uniqueidentifier] NOT NULL,
	[nombre] [varchar](50) NULL,
	[fichero] [varchar](300) NULL,
	[defecto] [bit] NULL,
 CONSTRAINT [PK_Plangeneral] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreferenciasUsuario]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreferenciasUsuario](
	[fkUsuario] [uniqueidentifier] NOT NULL,
	[tipo] [int] NOT NULL,
	[id] [varchar](100) NOT NULL,
	[nombre] [varchar](100) NOT NULL,
	[xml] [xml] NULL,
 CONSTRAINT [PK_PreferenciasUsuario] PRIMARY KEY CLUSTERED 
(
	[fkUsuario] ASC,
	[tipo] ASC,
	[id] ASC,
	[nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Presupuestos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Presupuestos](
	[empresa] [varchar](4) NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fkejercicio] [int] NOT NULL CONSTRAINT [DF__Presupues__ejerc__0F4D3C5F]  DEFAULT ((1)),
	[fkseries] [varchar](3) NOT NULL,
	[identificadorsegmento] [varchar](12) NOT NULL,
	[referencia] [varchar](30) NULL,
	[fechadocumento] [datetime] NULL,
	[fechavalidez] [datetime] NULL,
	[fechaentrega] [datetime] NULL,
	[fecharevision] [datetime] NULL,
	[fkalmacen] [int] NULL,
	[fkclientes] [varchar](15) NULL,
	[nombrecliente] [varchar](200) NULL,
	[clientedireccion] [varchar](100) NULL,
	[clientepoblacion] [varchar](100) NULL,
	[clientecp] [varchar](10) NULL,
	[clientepais] [varchar](50) NULL,
	[clienteprovincia] [varchar](50) NULL,
	[clientetelefono] [varchar](50) NULL,
	[clientefax] [varchar](15) NULL,
	[clienteemail] [varchar](200) NULL,
	[clientenif] [varchar](15) NULL,
	[fkformaspago] [int] NULL,
	[fkagentes] [varchar](15) NULL,
	[fkcomerciales] [varchar](15) NULL,
	[comisionagente] [float] NULL,
	[comisioncomercial] [float] NULL,
	[fkmonedas] [int] NULL,
	[cambioadicional] [float] NULL,
	[importebruto] [float] NULL,
	[importebaseimponible] [float] NULL,
	[importeportes] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[importetotaldoc] [float] NULL,
	[importetotalmonedabase] [float] NULL,
	[notas] [text] NULL,
	[fkestados] [varchar](10) NULL,
	[fkobras] [varchar](15) NULL,
	[incoterm] [varchar](3) NULL,
	[descripcionincoterm] [varchar](30) NULL,
	[peso] [int] NULL,
	[confianza] [int] NULL,
	[costemateriales] [float] NULL,
	[tiempooficinatecnica] [float] NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fktransportista] [varchar](15) NULL,
	[tipocambio] [float] NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[unidadnegocio] [varchar](3) NULL,
	[referenciadocumento] [varchar](30) NULL,
	[fkbancosmandatos] [varchar](3) NULL,
	[cartacredito] [varchar](25) NULL,
	[vencimientocartacredito] [datetime] NULL,
	[contenedores] [int] NULL,
	[fkcuentastesoreria] [varchar](15) NULL,
	[fkusuarioalta] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Presupues__fkusu__0B7CAB7B]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechaalta] [datetime] NOT NULL CONSTRAINT [DF__Presupues__fecha__0C70CFB4]  DEFAULT (getdate()),
	[fkusuariomodificacion] [uniqueidentifier] NOT NULL CONSTRAINT [DF__Presupues__fkusu__0D64F3ED]  DEFAULT ('00000000-0000-0000-0000-000000000000'),
	[fechamodificacion] [datetime] NOT NULL CONSTRAINT [DF__Presupues__fecha__0E591826]  DEFAULT (getdate()),
	[fkcarpetas] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Presupuestos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PresupuestosLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PresupuestosLin](
	[empresa] [varchar](4) NOT NULL,
	[fkpresupuestos] [int] NOT NULL,
	[id] [int] NOT NULL,
	[fkarticulos] [varchar](15) NULL,
	[descripcion] [varchar](120) NULL,
	[lote] [varchar](12) NULL,
	[tabla] [int] NULL,
	[cantidad] [float] NULL,
	[cantidadpedida] [float] NULL,
	[largo] [float] NULL,
	[ancho] [float] NULL,
	[grueso] [float] NULL,
	[fkunidades] [varchar](2) NULL,
	[metros] [float] NULL,
	[precio] [float] NULL,
	[porcentajedescuento] [float] NULL,
	[importedescuento] [float] NULL,
	[fktiposiva] [varchar](3) NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[cuotarecargoequivalencia] [float] NULL,
	[importe] [float] NULL,
	[notas] [text] NULL,
	[documentoorigen] [varchar](15) NULL,
	[documentodestino] [varchar](15) NULL,
	[canal] [varchar](3) NULL,
	[precioanterior] [float] NULL,
	[revision] [varchar](1) NULL,
	[decimalesmonedas] [int] NULL,
	[decimalesmedidas] [int] NULL,
	[labor1l1] [int] NULL,
	[labor2l1] [varchar](3) NULL,
	[labor3l1] [varchar](3) NULL,
	[labor4l1] [varchar](3) NULL,
	[labor1l2] [int] NULL,
	[labor2l2] [varchar](3) NULL,
	[labor3l2] [varchar](3) NULL,
	[labor4l2] [varchar](3) NULL,
	[labor1l3] [int] NULL,
	[labor2l3] [varchar](3) NULL,
	[labor3l3] [varchar](3) NULL,
	[labor4l3] [varchar](3) NULL,
	[labor1l4] [int] NULL,
	[labor2l4] [varchar](3) NULL,
	[labor3l4] [varchar](3) NULL,
	[labor4l4] [varchar](3) NULL,
	[bundle] [varchar](2) NULL,
	[tblnum] [int] NULL,
	[caja] [int] NULL,
 CONSTRAINT [PK_PresupuestosLin_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkpresupuestos] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PresupuestosTotales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PresupuestosTotales](
	[empresa] [varchar](4) NOT NULL,
	[fkpresupuestos] [int] NOT NULL CONSTRAINT [DF__Presupues__fkpre__19CACAD2]  DEFAULT ((1)),
	[fktiposiva] [varchar](3) NOT NULL,
	[brutototal] [float] NULL,
	[porcentajerecargoequivalencia] [float] NULL,
	[importerecargoequivalencia] [float] NULL,
	[porcentajedescuentoprontopago] [float] NULL,
	[importedescuentoprontopago] [float] NULL,
	[porcentajedescuentocomercial] [float] NULL,
	[importedescuentocomercial] [float] NULL,
	[basetotal] [float] NULL,
	[porcentajeiva] [float] NULL,
	[cuotaiva] [float] NULL,
	[subtotal] [float] NULL,
	[decimalesmonedas] [int] NULL,
 CONSTRAINT [PK_PresupuestosTotales_1] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkpresupuestos] ASC,
	[fktiposiva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Prospectos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Prospectos](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fkfamiliacliente] [varchar](3) NULL,
	[fkzonacliente] [varchar](3) NULL,
	[fktipoempresa] [varchar](3) NULL,
	[fkunidadnegocio] [varchar](3) NULL,
	[fkincoterm] [varchar](3) NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[fkidiomas] [varchar](50) NULL,
	[fkregimeniva] [varchar](5) NOT NULL,
	[fkformaspago] [int] NOT NULL,
	[fkmodocontacto] [varchar](3) NULL,
	[observaciones] [text] NULL,
	[fktarifas] [varchar](12) NULL,
	[fkmonedas] [varchar](3) NULL,
	[fkcuentasagente] [varchar](15) NULL,
	[fkcuentascomercial] [varchar](15) NULL,
 CONSTRAINT [PK_Prospectos] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Proveedores]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Proveedores](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fkidiomas] [varchar](3) NOT NULL,
	[fkfamiliaproveedor] [varchar](3) NULL,
	[fkzonaproveedor] [varchar](3) NULL,
	[fktipoempresa] [varchar](3) NULL,
	[fkunidadnegocio] [varchar](3) NULL,
	[fkincoterm] [varchar](3) NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[fkmonedas] [int] NOT NULL,
	[fkregimeniva] [varchar](5) NOT NULL,
	[fkgruposiva] [varchar](4) NULL,
	[criterioiva] [int] NOT NULL,
	[fktiposretencion] [varchar](4) NULL,
	[fktransportistahabitual] [varchar](15) NULL,
	[tipoportes] [int] NULL,
	[cuentatesoreria] [varchar](15) NULL,
	[fkformaspago] [int] NOT NULL,
	[descuentoprontopago] [float] NULL,
	[descuentocomercial] [float] NULL,
	[diafijopago1] [int] NULL,
	[diafijopago2] [int] NULL,
	[periodonopagodesde] [varchar](5) NULL,
	[periodonopagohasta] [varchar](5) NULL,
	[notas] [text] NULL,
	[tarifa] [varchar](50) NULL,
 CONSTRAINT [PK_Proveedores] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Provincias]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Provincias](
	[codigopais] [varchar](3) NOT NULL,
	[id] [varchar](2) NOT NULL,
	[nombre] [varchar](100) NULL,
 CONSTRAINT [PK_Provincias] PRIMARY KEY CLUSTERED 
(
	[codigopais] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Puertos]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Puertos](
	[fkpaises] [varchar](3) NOT NULL,
	[id] [varchar](4) NOT NULL,
	[descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_puertos] PRIMARY KEY CLUSTERED 
(
	[fkpaises] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RegimenIva]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RegimenIva](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](5) NOT NULL,
	[descripcion] [varchar](50) NULL,
	[normal] [bit] NULL,
	[recargo] [bit] NULL,
	[exportacion] [bit] NULL,
	[exentotasa] [bit] NULL,
	[operacionue] [bit] NULL,
	[inversionsujetopasivo] [bit] NULL,
	[operacionesnosujetas] [bit] NULL,
	[zonaespecial] [bit] NULL,
	[canariasigic] [bit] NULL,
	[extranjero] [bit] NULL,
	[ivadiferido] [bit] NULL,
	[ivaimportacion] [bit] NULL,
	[incompatiblecriteriocaja] [int] NULL,
	[soportadorepercutidoambos] [int] NULL,
	[bieninversion] [bit] NULL,
	[exentosventas] [bit] NULL,
	[claveoperacion340] [varchar](3) NULL,
	[incluirmodelo347] [bit] NULL,
 CONSTRAINT [PK_RegimenIva] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Roles](
	[id] [uniqueidentifier] NOT NULL,
	[role] [varchar](30) NULL,
	[permisos] [xml] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RolesUsuarios]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolesUsuarios](
	[FkUsuarios] [uniqueidentifier] NOT NULL,
	[FkRoles] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RolesUsuarios] PRIMARY KEY CLUSTERED 
(
	[FkUsuarios] ASC,
	[FkRoles] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Series]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Series](
	[empresa] [varchar](4) NOT NULL,
	[tipodocumento] [varchar](3) NOT NULL,
	[id] [varchar](3) NOT NULL,
	[descripcion] [varchar](50) NULL,
	[fkmonedas] [int] NULL,
	[fkregimeniva] [varchar](5) NULL,
	[fkcontadores] [varchar](12) NULL,
	[fkejercicios] [varchar](4) NULL,
	[tipoimpresion] [int] NULL,
	[riesgo] [bit] NULL,
	[exentoiva] [bit] NULL,
	[borrador] [bit] NULL,
	[rectificativa] [bit] NULL,
	[fkseriesasociada] [varchar](3) NULL,
	[fechamodificacionbloqueo] [datetime] NULL,
	[fkusuariobloqueo] [uniqueidentifier] NULL,
	[bloqueada] [bit] NULL,
	[fkmotivosbloqueo] [varchar](3) NULL,
 CONSTRAINT [PK_Series] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[tipodocumento] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tablasvarias]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tablasvarias](
	[id] [int] NOT NULL,
	[nombre] [varchar](100) NULL,
	[clase] [varchar](300) NULL,
	[tipo] [int] NULL,
	[noeditable] [bit] NULL,
 CONSTRAINT [PK_Tablasvarias_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TablasvariasLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TablasvariasLin](
	[fkTablasvarias] [int] NOT NULL,
	[id] [uniqueidentifier] NOT NULL,
	[xml] [xml] NULL,
 CONSTRAINT [PK_TablasvariasLin_1] PRIMARY KEY CLUSTERED 
(
	[fkTablasvarias] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tarifas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tarifas](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](12) NOT NULL,
	[descripcion] [varchar](40) NULL,
	[tipoflujo] [int] NULL,
	[tipotarifa] [int] NULL,
	[fkcuentas] [varchar](15) NULL,
	[fkmonedas] [int] NULL,
	[asignartarifaalcreararticulos] [bit] NULL,
	[precioobligatorio] [bit] NULL,
	[validodesde] [datetime] NULL,
	[validohasta] [datetime] NULL,
	[ivaincluido] [bit] NULL,
	[observaciones] [text] NULL,
	[precioautomaticobase] [varchar](12) NULL,
	[precioautomaticoporcentajebase] [float] NULL,
	[precioautomaticoporcentajefijo] [float] NULL,
	[precioautomaticofkfamiliasproductosdesde] [varchar](2) NULL,
	[precioautomaticofkfamiliasproductoshasta] [varchar](2) NULL,
	[precioautomaticofkmaterialesdesde] [varchar](3) NULL,
	[precioautomaticofkmaterialeshasta] [varchar](3) NULL,
	[fkMotivosbloqueo] [varchar](3) NULL,
	[fechamodificacionbloqueo] [datetime] NULL,
	[fkUsuariobloqueo] [uniqueidentifier] NULL,
	[bloqueada] [bit] NULL,
 CONSTRAINT [PK_Tarifas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tarifasbase]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tarifasbase](
	[fktarifa] [varchar](12) NOT NULL,
	[tipoflujo] [int] NOT NULL,
	[descripcion] [varchar](40) NULL,
 CONSTRAINT [PK_Tarifasbase_1] PRIMARY KEY CLUSTERED 
(
	[fktarifa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TarifasLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TarifasLin](
	[empresa] [varchar](4) NOT NULL,
	[fktarifas] [varchar](12) NOT NULL,
	[fkarticulos] [varchar](15) NOT NULL,
	[precio] [float] NULL,
	[descuento] [float] NULL,
	[Unidades] [varchar](2) NULL,
 CONSTRAINT [PK_TarifasLin] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fktarifas] ASC,
	[fkarticulos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tiposcuentas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tiposcuentas](
	[empresa] [varchar](4) NOT NULL,
	[tipos] [int] NOT NULL,
	[cuenta] [varchar](10) NULL,
	[descripcion] [varchar](100) NULL,
	[nifobligatorio] [bit] NULL,
	[categoria] [int] NOT NULL,
 CONSTRAINT [PK_Gestioncuentas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[tipos] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TiposcuentasLin]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TiposcuentasLin](
	[empresa] [varchar](4) NOT NULL,
	[fkTiposcuentas] [int] NOT NULL,
	[cuenta] [varchar](10) NOT NULL,
	[descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_GestioncuentasLin] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkTiposcuentas] ASC,
	[cuenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TiposIva]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TiposIva](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](3) NOT NULL,
	[nombre] [varchar](50) NULL,
	[fkgruposiva] [varchar](3) NULL,
	[porcentajeiva] [float] NULL,
	[porcentajerecargoequivalente] [float] NULL,
	[fkcuentasivasoportado] [varchar](10) NULL,
	[fkcuentasivarepercutido] [varchar](10) NULL,
	[fkcuentasivanodeducible] [varchar](10) NULL,
	[fkcuentasrecargoequivalenciarepercutido] [varchar](10) NULL,
	[fkcuentasivasoportadocriteriocaja] [varchar](10) NULL,
	[fkcuentasivarepercutidocriteriocaja] [varchar](10) NULL,
	[fkcuentasrecargoequivalenciarepercutidocriteriocaja] [varchar](10) NULL,
	[ivasoportado] [bit] NULL,
	[ivadeducible] [bit] NULL,
	[porcentajededucible] [float] NULL,
 CONSTRAINT [PK_TiposIva] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tiposretenciones]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tiposretenciones](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](4) NOT NULL,
	[descripcion] [varchar](100) NULL,
	[porcentajeretencion] [float] NULL,
	[fkcuentascargo] [varchar](15) NULL,
	[fkcuentasabono] [varchar](15) NULL,
	[tiporendimiento] [int] NULL,
 CONSTRAINT [PK_retenciones] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transportistas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transportistas](
	[empresa] [varchar](4) NOT NULL,
	[fkcuentas] [varchar](15) NOT NULL,
	[fkidiomas] [varchar](3) NOT NULL,
	[fkfamiliaacreedor] [varchar](3) NULL,
	[fkzonaacreedor] [varchar](3) NULL,
	[fktipoempresa] [varchar](3) NULL,
	[fkunidadnegocio] [varchar](3) NULL,
	[fkincoterm] [varchar](3) NULL,
	[fkpuertosfkpaises] [varchar](3) NULL,
	[fkpuertosid] [varchar](4) NULL,
	[fkmonedas] [int] NOT NULL,
	[fkregimeniva] [varchar](5) NOT NULL,
	[fkgruposiva] [varchar](4) NULL,
	[criterioiva] [int] NOT NULL,
	[fktiposretencion] [varchar](4) NULL,
	[fktransportistahabitual] [varchar](15) NULL,
	[tipoportes] [int] NULL,
	[cuentatesoreria] [varchar](15) NULL,
	[fkformaspago] [int] NOT NULL,
	[descuentoprontopago] [float] NULL,
	[descuentocomercial] [float] NULL,
	[diafijopago1] [int] NULL,
	[diafijopago2] [int] NULL,
	[periodonopagodesde] [varchar](5) NULL,
	[periodonopagohasta] [varchar](5) NULL,
	[notas] [text] NULL,
	[tarifa] [varchar](50) NULL,
	[conductorhabitual] [varchar](30) NULL,
	[matricula] [varchar](12) NULL,
	[remolque] [varchar](12) NULL,
	[tipotransporte] [varchar](3) NULL,
 CONSTRAINT [PK_Transportistas] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[fkcuentas] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Unidades]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Unidades](
	[empresa] [varchar](4) NOT NULL,
	[id] [varchar](2) NOT NULL,
	[codigounidad] [varchar](2) NULL,
	[descripcion] [varchar](100) NULL,
	[descripcion2] [varchar](100) NULL,
	[textocorto] [varchar](10) NULL,
	[textocorto2] [varchar](10) NULL,
	[formula] [int] NOT NULL,
	[tiposmovimientostock] [int] NOT NULL,
	[tipostock] [int] NOT NULL,
	[tipototal] [int] NOT NULL,
	[decimalestotales] [int] NOT NULL,
 CONSTRAINT [PK_Unidades] PRIMARY KEY CLUSTERED 
(
	[empresa] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Usuarios](
	[id] [uniqueidentifier] NOT NULL,
	[usuario] [varchar](20) NOT NULL,
	[password] [varchar](15) NOT NULL,
	[puedecambiarempresa] [bit] NULL,
	[usuariomail] [varchar](100) NULL,
	[passwordmail] [varchar](100) NULL,
	[smtp] [varchar](400) NULL,
	[puerto] [int] NULL,
	[ssl] [bit] NULL,
	[email] [varchar](400) NULL,
	[firma] [text] NULL,
	[nombre] [varchar](200) NULL,
	[copiaremitente] [int] NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[Canales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Canales]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '30')

GO
/****** Object:  View [dbo].[Familiamateriales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Familiamateriales]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '20')

GO
/****** Object:  View [dbo].[FamiliasClientes]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[FamiliasClientes]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '1')

GO
/****** Object:  View [dbo].[Gruposmateriales]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Gruposmateriales]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '21')

GO
/****** Object:  View [dbo].[Paises]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Paises]
AS
SELECT     xml.value('(/TablasVariasPaisesModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasPaisesModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasPaisesModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2, 
                      xml.value('(/TablasVariasPaisesModel/CodigoIsoAlfa2/text())[1]', 'varchar(40)') AS CodigoIsoAlfa2, xml.value('(/TablasVariasPaisesModel/CodigoIsoAlfa3/text())[1]', 
                      'varchar(40)') AS CodigoIsoAlfa3, xml.value('(/TablasVariasPaisesModel/CodigoIsoNumerico/text())[1]', 'varchar(40)') AS CodigoIsoNumerico, 
                      xml.value('(/TablasVariasPaisesModel/NifEuropeo/text())[1]', 'varchar(40)') AS NifEuropeo
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '3166')

GO
/****** Object:  View [dbo].[Tiposempresas]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Tiposempresas]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '2023')

GO
/****** Object:  View [dbo].[Unidadesnegocio]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Unidadesnegocio]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '15')

GO
/****** Object:  View [dbo].[ZonasClientes]    Script Date: 07/02/2017 13:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ZonasClientes]
AS
SELECT     xml.value('(/TablasVariasGeneralModel/Valor/text())[1]', 'varchar(40)') AS Valor, xml.value('(/TablasVariasGeneralModel/Descripcion/text())[1]', 'varchar(40)') 
                      AS Descripcion, xml.value('(/TablasVariasGeneralModel/Descripcion2/text())[1]', 'varchar(40)') AS Descripcion2
FROM         dbo.TablasvariasLin
WHERE     (fkTablasvarias = '2')

GO
ALTER TABLE [dbo].[AlbaranesLin]  WITH CHECK ADD  CONSTRAINT [FK_AlbaranesLin_Albaranes] FOREIGN KEY([empresa], [fkalbaranes])
REFERENCES [dbo].[Albaranes] ([empresa], [id])
GO
ALTER TABLE [dbo].[AlbaranesLin] CHECK CONSTRAINT [FK_AlbaranesLin_Albaranes]
GO
ALTER TABLE [dbo].[AlbaranesTotales]  WITH CHECK ADD  CONSTRAINT [FK_AlbaranesTotales_Albaranes] FOREIGN KEY([empresa], [fkalbaranes])
REFERENCES [dbo].[Albaranes] ([empresa], [id])
GO
ALTER TABLE [dbo].[AlbaranesTotales] CHECK CONSTRAINT [FK_AlbaranesTotales_Albaranes]
GO
ALTER TABLE [dbo].[AppPermisosRoles]  WITH CHECK ADD  CONSTRAINT [FK_AppPermisosRoles_AppPermisos] FOREIGN KEY([fkAppPermisos])
REFERENCES [dbo].[AppPermisos] ([id])
GO
ALTER TABLE [dbo].[AppPermisosRoles] CHECK CONSTRAINT [FK_AppPermisosRoles_AppPermisos]
GO
ALTER TABLE [dbo].[AppPermisosRoles]  WITH CHECK ADD  CONSTRAINT [FK_AppPermisosRoles_Roles] FOREIGN KEY([fkRoles])
REFERENCES [dbo].[Roles] ([id])
GO
ALTER TABLE [dbo].[AppPermisosRoles] CHECK CONSTRAINT [FK_AppPermisosRoles_Roles]
GO
ALTER TABLE [dbo].[AppPermisosUsuarios]  WITH CHECK ADD  CONSTRAINT [FK_AppPermisosUsuarios_AppPermisos] FOREIGN KEY([fkAppPermisos])
REFERENCES [dbo].[AppPermisos] ([id])
GO
ALTER TABLE [dbo].[AppPermisosUsuarios] CHECK CONSTRAINT [FK_AppPermisosUsuarios_AppPermisos]
GO
ALTER TABLE [dbo].[AppPermisosUsuarios]  WITH CHECK ADD  CONSTRAINT [FK_AppPermisosUsuarios_Usuarios] FOREIGN KEY([fkUsuarios])
REFERENCES [dbo].[Usuarios] ([id])
GO
ALTER TABLE [dbo].[AppPermisosUsuarios] CHECK CONSTRAINT [FK_AppPermisosUsuarios_Usuarios]
GO
ALTER TABLE [dbo].[CaracteristicasLin]  WITH CHECK ADD  CONSTRAINT [FK_CaracteristicasLin_Caracteristicas] FOREIGN KEY([empresa], [fkcaracteristicas])
REFERENCES [dbo].[Caracteristicas] ([empresa], [id])
GO
ALTER TABLE [dbo].[CaracteristicasLin] CHECK CONSTRAINT [FK_CaracteristicasLin_Caracteristicas]
GO
ALTER TABLE [dbo].[ContadoresLin]  WITH CHECK ADD  CONSTRAINT [FK_ContadoresLin_Contadores] FOREIGN KEY([empresa], [fkcontadores])
REFERENCES [dbo].[Contadores] ([empresa], [id])
GO
ALTER TABLE [dbo].[ContadoresLin] CHECK CONSTRAINT [FK_ContadoresLin_Contadores]
GO
ALTER TABLE [dbo].[CriteriosagrupacionLin]  WITH CHECK ADD  CONSTRAINT [FK_CriteriosagrupacionLin_Criteriosagrupacion] FOREIGN KEY([fkcriteriosagrupacion])
REFERENCES [dbo].[Criteriosagrupacion] ([id])
GO
ALTER TABLE [dbo].[CriteriosagrupacionLin] CHECK CONSTRAINT [FK_CriteriosagrupacionLin_Criteriosagrupacion]
GO
ALTER TABLE [dbo].[FacturasLin]  WITH CHECK ADD  CONSTRAINT [FK_FacturasLin_Facturas] FOREIGN KEY([empresa], [fkfacturas])
REFERENCES [dbo].[Facturas] ([empresa], [id])
GO
ALTER TABLE [dbo].[FacturasLin] CHECK CONSTRAINT [FK_FacturasLin_Facturas]
GO
ALTER TABLE [dbo].[FacturasTotales]  WITH CHECK ADD  CONSTRAINT [FK_FacturasTotales_Facturas] FOREIGN KEY([empresa], [fkfacturas])
REFERENCES [dbo].[Facturas] ([empresa], [id])
GO
ALTER TABLE [dbo].[FacturasTotales] CHECK CONSTRAINT [FK_FacturasTotales_Facturas]
GO
ALTER TABLE [dbo].[FacturasVencimientos]  WITH CHECK ADD  CONSTRAINT [FK_FacturasVencimientos_Facturas] FOREIGN KEY([empresa], [fkfacturas])
REFERENCES [dbo].[Facturas] ([empresa], [id])
GO
ALTER TABLE [dbo].[FacturasVencimientos] CHECK CONSTRAINT [FK_FacturasVencimientos_Facturas]
GO
ALTER TABLE [dbo].[Ficheros]  WITH CHECK ADD  CONSTRAINT [FK_Ficheros_Carpetas] FOREIGN KEY([empresa], [fkcarpetas])
REFERENCES [dbo].[Carpetas] ([empresa], [id])
GO
ALTER TABLE [dbo].[Ficheros] CHECK CONSTRAINT [FK_Ficheros_Carpetas]
GO
ALTER TABLE [dbo].[FormasPagoLin]  WITH CHECK ADD  CONSTRAINT [FK_FormasPagoLin_FormasPago] FOREIGN KEY([fkFormasPago])
REFERENCES [dbo].[FormasPago] ([id])
GO
ALTER TABLE [dbo].[FormasPagoLin] CHECK CONSTRAINT [FK_FormasPagoLin_FormasPago]
GO
ALTER TABLE [dbo].[GruposIvaLin]  WITH CHECK ADD  CONSTRAINT [FK_GruposIvaLin_GruposIva] FOREIGN KEY([empresa], [fkgruposiva])
REFERENCES [dbo].[GruposIva] ([empresa], [id])
GO
ALTER TABLE [dbo].[GruposIvaLin] CHECK CONSTRAINT [FK_GruposIvaLin_GruposIva]
GO
ALTER TABLE [dbo].[GuiascontablesLin]  WITH CHECK ADD  CONSTRAINT [FK_GuiascontablesLin_Guiascontables] FOREIGN KEY([empresa], [fkguiascontables])
REFERENCES [dbo].[Guiascontables] ([empresa], [id])
GO
ALTER TABLE [dbo].[GuiascontablesLin] CHECK CONSTRAINT [FK_GuiascontablesLin_Guiascontables]
GO
ALTER TABLE [dbo].[MaterialesLin]  WITH CHECK ADD  CONSTRAINT [FK_MaterialesLin_Materiales] FOREIGN KEY([empresa], [fkmateriales])
REFERENCES [dbo].[Materiales] ([empresa], [id])
GO
ALTER TABLE [dbo].[MaterialesLin] CHECK CONSTRAINT [FK_MaterialesLin_Materiales]
GO
ALTER TABLE [dbo].[MonedasLog]  WITH CHECK ADD  CONSTRAINT [FK_MonedasLog_Monedas] FOREIGN KEY([fkMonedas])
REFERENCES [dbo].[Monedas] ([id])
GO
ALTER TABLE [dbo].[MonedasLog] CHECK CONSTRAINT [FK_MonedasLog_Monedas]
GO
ALTER TABLE [dbo].[PedidosLin]  WITH CHECK ADD  CONSTRAINT [FK_PedidosLin_Pedidos] FOREIGN KEY([empresa], [fkpedidos])
REFERENCES [dbo].[Pedidos] ([empresa], [id])
GO
ALTER TABLE [dbo].[PedidosLin] CHECK CONSTRAINT [FK_PedidosLin_Pedidos]
GO
ALTER TABLE [dbo].[PedidosTotales]  WITH CHECK ADD  CONSTRAINT [FK_PedidosTotales_Pedidos] FOREIGN KEY([empresa], [fkpedidos])
REFERENCES [dbo].[Pedidos] ([empresa], [id])
GO
ALTER TABLE [dbo].[PedidosTotales] CHECK CONSTRAINT [FK_PedidosTotales_Pedidos]
GO
ALTER TABLE [dbo].[Presupuestos]  WITH CHECK ADD  CONSTRAINT [FK_Presupuestos_Presupuestos] FOREIGN KEY([empresa], [id])
REFERENCES [dbo].[Presupuestos] ([empresa], [id])
GO
ALTER TABLE [dbo].[Presupuestos] CHECK CONSTRAINT [FK_Presupuestos_Presupuestos]
GO
ALTER TABLE [dbo].[PresupuestosLin]  WITH CHECK ADD  CONSTRAINT [FK_PresupuestosLin_Presupuestos] FOREIGN KEY([empresa], [fkpresupuestos])
REFERENCES [dbo].[Presupuestos] ([empresa], [id])
GO
ALTER TABLE [dbo].[PresupuestosLin] CHECK CONSTRAINT [FK_PresupuestosLin_Presupuestos]
GO
ALTER TABLE [dbo].[PresupuestosTotales]  WITH CHECK ADD  CONSTRAINT [FK_PresupuestosTotales_Presupuestos] FOREIGN KEY([empresa], [fkpresupuestos])
REFERENCES [dbo].[Presupuestos] ([empresa], [id])
GO
ALTER TABLE [dbo].[PresupuestosTotales] CHECK CONSTRAINT [FK_PresupuestosTotales_Presupuestos]
GO
ALTER TABLE [dbo].[RolesUsuarios]  WITH CHECK ADD  CONSTRAINT [FK_RolesUsuarios_Roles] FOREIGN KEY([FkRoles])
REFERENCES [dbo].[Roles] ([id])
GO
ALTER TABLE [dbo].[RolesUsuarios] CHECK CONSTRAINT [FK_RolesUsuarios_Roles]
GO
ALTER TABLE [dbo].[RolesUsuarios]  WITH CHECK ADD  CONSTRAINT [FK_RolesUsuarios_Usuarios] FOREIGN KEY([FkUsuarios])
REFERENCES [dbo].[Usuarios] ([id])
GO
ALTER TABLE [dbo].[RolesUsuarios] CHECK CONSTRAINT [FK_RolesUsuarios_Usuarios]
GO
ALTER TABLE [dbo].[TablasvariasLin]  WITH CHECK ADD  CONSTRAINT [FK_TablasvariasLin_Tablasvarias] FOREIGN KEY([fkTablasvarias])
REFERENCES [dbo].[Tablasvarias] ([id])
GO
ALTER TABLE [dbo].[TablasvariasLin] CHECK CONSTRAINT [FK_TablasvariasLin_Tablasvarias]
GO
ALTER TABLE [dbo].[TarifasLin]  WITH CHECK ADD  CONSTRAINT [FK_TarifasLin_TarifasLin] FOREIGN KEY([empresa], [fktarifas])
REFERENCES [dbo].[Tarifas] ([empresa], [id])
GO
ALTER TABLE [dbo].[TarifasLin] CHECK CONSTRAINT [FK_TarifasLin_TarifasLin]
GO
ALTER TABLE [dbo].[TiposcuentasLin]  WITH CHECK ADD  CONSTRAINT [FK_GestioncuentasLin_Gestioncuentas] FOREIGN KEY([empresa], [fkTiposcuentas])
REFERENCES [dbo].[Tiposcuentas] ([empresa], [tipos])
GO
ALTER TABLE [dbo].[TiposcuentasLin] CHECK CONSTRAINT [FK_GestioncuentasLin_Gestioncuentas]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Canales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Canales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Familiamateriales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Familiamateriales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FamiliasClientes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'FamiliasClientes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Gruposmateriales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Gruposmateriales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 2340
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Paises'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Paises'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Tiposempresas'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Tiposempresas'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Unidadesnegocio'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Unidadesnegocio'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TablasvariasLin"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 99
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ZonasClientes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ZonasClientes'
GO
