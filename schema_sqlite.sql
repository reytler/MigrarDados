CREATE TABLE
    'tab_cnae' (
        'cod_secao' nvarchar (1073741823),
        'nm_secao' nvarchar (1073741823),
        'cod_divisao' nvarchar (1073741823),
        'nm_divisao' nvarchar (1073741823),
        'cod_grupo' nvarchar (1073741823),
        'nm_grupo' nvarchar (1073741823),
        'cod_classe' nvarchar (1073741823),
        'nm_classe' nvarchar (1073741823),
        'cod_cnae' nvarchar (1073741823),
        'nm_cnae' nvarchar (1073741823)
    );

CREATE TABLE
    'tab_codio_municipios_siafi' (
        'codigo_siafi' nvarchar (1073741823),
        'cnpj' nvarchar (1073741823),
        'descricao' nvarchar (1073741823),
        'uf' nvarchar (1073741823),
        'codigo_ibge' nvarchar (1073741823)
    );

CREATE TABLE
    'tab_natureza_juridica' (
        'cod_natureza_juridica' nvarchar (1073741823),
        'nm_natureza_juridica' nvarchar (1073741823),
        'cod_subclass_natureza_juridica' nvarchar (1073741823),
        'nm_subclass_natureza_juridica' nvarchar (1073741823)
    );

CREATE TABLE
    'cnpj_dados_cadastrais_pj' (
        'tipo_de_registro' nvarchar (1073741823),
        'indicador' nvarchar (1073741823),
        'tipo_atualizacao' nvarchar (1073741823),
        'cnpj' nvarchar (1073741823),
        'identificador_matriz_filial' nvarchar (1073741823),
        'razao_social' nvarchar (1073741823),
        'nome_fantasia' nvarchar (1073741823),
        'situacao_cadastral' nvarchar (1073741823),
        'data_situacao_cadastral' nvarchar (1073741823),
        'motivo_situacao_cadastral' nvarchar (1073741823),
        'nm_cidade_exterior' nvarchar (1073741823),
        'cod_pais' nvarchar (1073741823),
        'nm_pais' nvarchar (1073741823),
        'codigo_natureza_juridica' nvarchar (1073741823),
        'data_inicio_atividade' nvarchar (1073741823),
        'cnae_fiscal' nvarchar (1073741823),
        'descricao_tipo_logradouro' nvarchar (1073741823),
        'logradouro' nvarchar (1073741823),
        'numero' nvarchar (1073741823),
        'complemento' nvarchar (1073741823),
        'bairro' nvarchar (1073741823),
        'cep' nvarchar (1073741823),
        'uf' nvarchar (1073741823),
        'codigo_municipio' nvarchar (1073741823),
        'municipio' nvarchar (1073741823),
        'ddd_telefone_1' nvarchar (1073741823),
        'ddd_telefone_2' nvarchar (1073741823),
        'ddd_fax' nvarchar (1073741823),
        'correio_eletronico' nvarchar (1073741823),
        'qualificacao_responsavel' nvarchar (1073741823),
        'capital_social_empresa' double,
        'porte_empresa' nvarchar (1073741823),
        'opcao_pelo_simples' nvarchar (1073741823),
        'data_opcao_pelo_simples' nvarchar (1073741823),
        'data_exclusao_simples' nvarchar (1073741823),
        'opcao_pelo_mei' nvarchar (1073741823),
        'situacao_especial' nvarchar (1073741823),
        'data_situacao_especial' nvarchar (1073741823),
        'filler' nvarchar (1073741823),
        'fim_registro' nvarchar (1073741823)
    );

CREATE TABLE
    'cnpj_dados_cnae_secundario_pj' (
        'tipo_de_registro' nvarchar (1073741823),
        'indicador' nvarchar (1073741823),
        'tipo_atualizacao' nvarchar (1073741823),
        'cnpj' nvarchar (1073741823),
        'cnae_secundario' nvarchar (1073741823),
        'filler' nvarchar (1073741823)
    );

CREATE TABLE
    'cnpj_dados_socios' (
        'tipo_de_registro' nvarchar (1073741823),
        'indicador' nvarchar (1073741823),
        'tipo_atualizacao' nvarchar (1073741823),
        'cnpj' nvarchar (1073741823),
        'identificador_socio' nvarchar (1073741823),
        'nome_socio' nvarchar (1073741823),
        'cnpj_cpf_socio' nvarchar (1073741823),
        'cod_qualificacao_socio' nvarchar (1073741823),
        'percentual_capital_socio' nvarchar (1073741823),
        'data_entrada_sociedade' nvarchar (1073741823),
        'cod_pais' nvarchar (1073741823),
        'nome_pais_socio' nvarchar (1073741823),
        'cpf_representante_legal' nvarchar (1073741823),
        'nome_representante' nvarchar (1073741823),
        'cod_qualificacao_representante_legal' nvarchar (1073741823),
        'fillter' nvarchar (1073741823),
        'fim_registro' nvarchar (1073741823)
    );

CREATE TABLE
    'caged' (
        'secao' nvarchar (254),
        'cd_municipio' nvarchar (255),
        'municipio' nvarchar (255),
        'uf' nvarchar (255),
        'faixa_empregados' varchar(254),
        'competencia' date,
        'fluxo' bigint
    );