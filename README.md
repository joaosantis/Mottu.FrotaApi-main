🛵 Desafio Mottu - Monitoramento de Pátio em Tempo Real

Este projeto é uma solução de ponta-a-ponta para o Desafio Mottu, como parte da disciplina de "Disruptive Architectures". O sistema monitoriza um pátio de motocicletas em tempo real, integrando Visão Computacional, uma API .NET e um dashboard web.

## 1\. Tecnologias Utilizadas

O projeto é uma arquitetura de micro-serviços que corre 5 componentes em paralelo:

  * **Backend (API):** .NET 9 (C\#) a correr na porta `5000`.
  * **Banco de Dados:** PostgreSQL a correr num container Docker na porta `5432`.
  * **Serviço de Visão (YOLO):** Python 3 (Google Colab) a usar `YOLOv8` e `OpenCV` para detetar motos e enviar localizações.
  * **Serviço de IoT (Simulador):** Python 3 (Google Colab) a usar `threading` e `requests` para enviar atualizações de status (ex: "Em Manutenção").
  * **Frontend (Dashboard):** HTML5, CSS3, e JavaScript (servido via `python -m http.server` na porta `8000`).
  * **Rede (Túnel):** Ngrok para expor a API local (porta `5000`) para o Colab.

## 2\. Resultados Finais

O projeto entrega uma solução "ponta-a-ponta" totalmente funcional:

1.  **Criação (YOLO):** O script de Visão Computacional analisa um vídeo, deteta objetos (motos/carros) e **envia** os dados (`moto_id`, `posicao_x`) para a API .NET. A API **salva** esta nova moto no banco de dados PostgreSQL.
2.  **Atualização (IoT):** O script de IoT (em paralelo) **envia** atualizações de status (`status`) para a API .NET. A API **atualiza** o registo da moto correspondente no banco.
3.  **Leitura (Dashboard):** O `index.html` (dashboard) **consulta** a API .NET a cada 5 segundos, que **lê** o estado atual de todas as motos no banco e o retorna. O dashboard desenha visualmente a posição e o status (cor) de cada moto.

## 3\. Instruções de Uso

Para executar o projeto completo, é necessário iniciar os 5 componentes.

### O Roteiro de Execução (5 Terminais)

**Terminal 1️⃣: Banco de Dados (Docker)**

1.  Abra o Docker Desktop e espere que ele inicie.
2.  Num terminal, inicie o container do PostgreSQL:
    ```bash
    docker start mottu-postgres
    ```

**Terminal 2️⃣: API .NET (Servidor)**

1.  Num terminal, navegue até à pasta de "publicação" da API (o nosso código corrigido):
    ```bash
    cd C:\Users\João\Downloads\Mottu.FrotaApi-main\Mottu.FrotaApi-main\Mottu.FrotaApi\bin\Debug\net9.0\publish
    ```
2.  Execute a API (ela irá rodar na porta `5000`):
    ```bash
    dotnet Mottu.FrotaApi.dll
    ```
3.  **Deixe este terminal aberto.**

**Terminal 3️⃣: Túnel (Ngrok)**

1.  Num terminal, navegue até à pasta do Ngrok:
    ```bash
    E:
    cd E:\ngrok
    ```
2.  Inicie o túnel a apontar para a porta correta da API (**5000**):
    ```bash
    .\ngrok.exe http 5000
    ```
3.  O `ngrok` irá gerar um novo link `https://...`. **Copie este link.**
4.  **Deixe este terminal aberto.**

**Terminal 4️⃣: Dashboard (Frontend)**

1.  Num terminal, navegue até à pasta do seu dashboard:
    ```bash
    E:
    cd E:\Dashboard
    ```
2.  Inicie o servidor web do Python (ele irá rodar na porta `8000`):
    ```bash
    python -m http.server 8000
    ```
3.  **Deixe este terminal aberto.**

**Componente 5️⃣: Simuladores (Google Colab)**

1.  Abra o seu **Notebook Unificado** (`yolo_+_simulador_iot.py`).
2.  **Atualize os Links:**
      * No **Google Colab (Célula 2)**: Cole o **novo link** do Ngrok (do Passo 3) na variável `NGROK_BASE_URL`.
      * No seu **`index.html`** (na pasta `E:\Dashboard`): Abra-o no VS Code e cole o **novo link** do Ngrok na variável `API_URL` (linha 97), garantindo que termina com `/api/patio/visualizar`. (Salve o ficheiro após a alteração).
3.  **Carregue o Vídeo:** Na barra de ficheiros à esquerda do Colab, faça o upload do `video_teste_facil.mp4`.
4.  Execute a **Célula 1 (`!pip install...`)**.
5.  Execute a **Célula 2 (Script Unificado)**.

**Componente 6️⃣: Visualização**
Abra o seu navegador e aceda a: http://localhost:8000
O pátio irá carregar e (após os 30s de atraso do IoT) começará a ser preenchido com as motos detectadas pelo YOLO e atualizadas pelo IoT.

1.  Abra o seu navegador e aceda a:
    **`http://localhost:8000`**
2.  O pátio irá carregar e (após os 30s de atraso do IoT) começará a ser preenchido com as motos detectadas pelo YOLO e atualizadas pelo IoT.
