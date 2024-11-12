
# myLocalNot

O **myLocalNot** é uma aplicação desenvolvida em .NET MAUI que exibe notificações locais com atualizações periódicas de clima, utilizando abordagens específicas para iOS e Android para garantir que as notificações sejam exibidas em segundo plano.

## Estrutura do Projeto

1. **MauiProgram.cs**
   - Configura a aplicação, incluindo a integração com o `Plugin.LocalNotification` para o envio de notificações locais.

2. **AppDelegate.cs (iOS)**
   - Configura o **Background Fetch** para realizar atualizações periódicas de dados de clima e exibir notificações no iOS.
   - Solicita permissões para notificações locais e define o intervalo mínimo para atualizações em segundo plano.

3. **MainActivity.cs (Android)**
   - Configura o **Foreground Service** e o **WorkManager** para manter a aplicação ativa em segundo plano e garantir notificações periódicas.
   - Agenda o `WeatherWorker` para obter dados de clima a cada 1 minuto.
   - Inicia o `WeatherForegroundService`, que permite que a aplicação se mantenha ativa em segundo plano.

4. **WeatherForegroundService.cs (Android)**
   - Este serviço em primeiro plano mantém a aplicação em execução contínua no Android, garantindo que as atualizações de clima sejam feitas regularmente, mesmo que o utilizador não esteja com a aplicação aberta.
   - Exibe uma notificação persistente que informa que a aplicação está ativa e a recolher dados meteorológicos.

5. **WeatherService.cs**
   - Serviço responsável por realizar chamadas HTTP para obter dados de clima, como temperatura e condições meteorológicas, através de uma API.
   - Utiliza `HttpClient` e `System.Text.Json` para buscar e processar os dados.

6. **WeatherWorker.cs (Android)**
   - Executa tarefas periódicas em segundo plano no Android, acionando o `WeatherService` para obter dados de clima e exibir notificações com as informações obtidas.

7. **Plugin.LocalNotification**
   - Gere as notificações locais em ambas as plataformas, permitindo configurar o título, a descrição e os dados de clima apresentados nas notificações.

## Funcionamento Geral

- **iOS**: O **Background Fetch** ativa periodicamente a aplicação para obter dados de clima. No `AppDelegate`, o `WeatherService` é chamado para recolher as informações meteorológicas, que são exibidas como notificações locais.

- **Android**: O **Foreground Service** e o **WorkManager** garantem que a aplicação se mantenha ativa em segundo plano. O `WeatherWorker` é acionado em intervalos de 1 minuto para recolher os dados de clima através do `WeatherService` e exibir notificações locais com as informações meteorológicas. O `WeatherForegroundService` mantém o aplicativo ativo, exibindo uma notificação persistente informando que a aplicação está a monitorizar o clima.

O **myLocalNot** foi projetado para fornecer atualizações de clima em segundo plano, mantendo os utilizadores informados sobre as condições meteorológicas através de notificações locais em iOS e Android.

---

## Configuração da Chave de API

A aplicação utiliza uma API de clima, como a OpenWeatherMap, para buscar dados meteorológicos. É necessário configurar a chave de API no código.

1. **Obter a Chave de API**:
   - Registe-se no [OpenWeatherMap](https://openweathermap.org/api) (ou noutra API de clima de sua preferência) e obtenha uma chave de API.

2. **Inserir a Chave no Código**:
   - No arquivo `WeatherService.cs`, localize a variável `ApiKey` e substitua `YOUR_API_KEY` pela chave obtida:

     private const string ApiKey = "YOUR_API_KEY"; // Substitua pela sua chave de API
   

3. **Salvar as Alterações**:
   - Após inserir a chave, salve as alterações. Agora a aplicação está configurada para acessar a API de clima.

---

## Configuração do Timing de Notificações

Para configurar o timing das notificações para 5 minutos, altere o código para garantir que `TimeSpan.FromMinutes(5)` seja usado em todas as configurações de agendamento. Isso inclui o agendamento de tarefas no `WeatherWorker` para Android ou qualquer método de temporizador usado.

### Exemplo de Configuração do Timing para 5 Minutos:

var workRequest = PeriodicWorkRequest.Builder
    .From<WeatherWorker>(TimeSpan.FromMinutes(5))
    .Build();

## Resumo

- **Chave de API**: Configure no `WeatherService.cs` para permitir o acesso à API de clima.
- **Timing de Notificações**: Substitua `TimeSpan.FromMinutes(1)` por `TimeSpan.FromMinutes(5)` para definir um intervalo de 5 minutos para as notificações.
