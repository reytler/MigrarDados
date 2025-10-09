# 🚀 MigrarDados — Migração de Base SQLite para MySQL com Entity Framework Core

Este projeto é um **utilitário de console** desenvolvido em **.NET 9** que realiza a migração de dados de um banco **SQLite** para um banco **MySQL**, utilizando **Entity Framework Core** com leitura em stream, processamento em lotes (`batch`) e transações para garantir performance e integridade.

* [Link do dataset(Kaggle)](https://www.kaggle.com/datasets/marcilonsilvacunha/amostracnpj)

---

## 📦 Funcionalidades

- Conexão direta com bancos **SQLite** e **MySQL**.  
- Migração de dados de tabelas do sqlite.  
- Processamento em **lotes configuráveis** (default: `5000`).  
- Controle de memória e performance via `ChangeTracker.Clear()`.  
- **Transação por lote** para garantir consistência.  
- **Retry automático** em falhas de inserção.  
- **Logs de progresso e tempo total** de execução.

---

## 🧩 Durante a execução, podemos ver logs como:


- ✅ Inseridos 5.000 registros até agora (00:10).
- ✅ Inseridos 10.000 registros até agora (00:20).
- 🎉 Sincronização concluída!
- 📊 Total inserido: 100.000 registros em 03:40.


## 🧠 Estratégias de Otimização

- AsNoTracking() para leitura mais leve do SQLite.

- AutoDetectChangesEnabled = false para reduzir CPU.

- ChangeTracker.Clear() após cada lote (libera memória).

- Uso de transações para reduzir commits.

- Retry automático com backoff exponencial em caso de erro.

- Stopwatch para medir tempo e progresso da migração.

## 🧰 Tecnologias Utilizadas
- .NET 9.0

- Entity Framework Core 9

- SQLite

- MySQL

## Utilização de streams:
### 🧠 Contexto
* No código que obtém os dados do Sqlite:
```csharp
    await foreach (var item in sqliteContext.CagedSQLite.AsNoTracking().AsAsyncEnumerable())
    {
        // ...
    }
```

* 💡 O que isso faz?
- Esse trecho usa AsAsyncEnumerable(), que é a forma do Entity Framework Core expor os resultados de uma query como um stream assíncrono (```IAsyncEnumerable<T>```).

Ou seja:

O EF não carrega toda a tabela caged de uma vez na memória.

Ele busca e processa registro por registro (ou pequenos blocos internos) conforme o await foreach avança.

Isso é o equivalente a um cursor no banco — leitura sob demanda.

👉 Essa é uma forma de streaming (fluxo de dados assíncrono) e é a maneira mais eficiente possível de percorrer um dataset muito grande no EF Core.

⚙️ Fluxo real no seu código

1. sqliteContext.CagedSQLite.AsNoTracking()
→ Desabilita o rastreamento de entidades (reduz memória e CPU).

2. .AsAsyncEnumerable()
→ Retorna um fluxo assíncrono (stream de dados).

3. await foreach (var item in ...)
→ Itera um registro por vez, sem carregar o resto do banco.

Dentro do loop, você adiciona ao batch, e ao atingir o limite, grava no MySQL.

## ✅ Benefícios do streaming aqui
* Baixo consumo de memória: O EF Core só mantém alguns registros na RAM por vez.
* Alta escalabilidade: Permite migrar milhões de linhas sem travar o processo.
* Fluxo contínuo: O SQLite é lido ao mesmo tempo em que o MySQL é escrito.
* Compatível com async/await: O loop não bloqueia a thread principal.

# Resultado
* Tabela caged migrada
* **14.552.432** de registros migrados em **42m 46s**
- O projeto usa stream aliado ao processamento por lote - mais precisamente, um stream assíncrono com IAsyncEnumerable.
Isso garante que possamos migrar até milhões de registros com uso de memória constante e controlado.

# Próximos passos
## - Realizar as operações nas outras tabelas do .sqlite de forma paralela, visando migrar os dados de uma tabela por Thread