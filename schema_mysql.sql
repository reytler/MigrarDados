-- cnpj.caged definição

CREATE TABLE `caged` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Secao` varchar(10) DEFAULT NULL,
  `CdMunicipio` varchar(20) DEFAULT NULL,
  `Municipio` varchar(255) DEFAULT NULL,
  `Uf` varchar(2) DEFAULT NULL,
  `FaixaEmpregados` varchar(50) DEFAULT NULL,
  `Competencia` date DEFAULT NULL,
  `Fluxo` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12350015 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;