🛵 Desafio Mottu - Monitoramento de Pátio em Tempo Real
Este projeto é uma solução de ponta-a-ponta para o Desafio Mottu, como parte da disciplina de "Disruptive Architectures". O objetivo é criar um sistema capaz de monitorizar um pátio de motocicletas, utilizando Visão Computacional (YOLO) para deteção de localização e simuladores de IoT para atualização de status, exibindo tudo em um dashboard em tempo real.

1. Tecnologias Utilizadas
O projeto é uma arquitetura de micro-serviços que corre 5 componentes em paralelo:

Backend (API): .NET 9 (C#) a correr na porta 5000.

Banco de Dados: PostgreSQL a correr num container Docker na porta 5432.

Serviço de Visão (YOLO): Python 3 (Google Colab) a usar YOLOv8 e OpenCV para detetar motos e enviar localizações.

Serviço de IoT (Simulador): Python 3 (Google Colab) a usar threading e requests para enviar atualizações de status (ex: "Em Manutenção").

Frontend (Dashboard): HTML5, CSS3, e JavaScript (servido via python -m http.server na porta 8000).

Rede (Túnel): Ngrok para expor a API local (porta 5000) para o Colab.

2. Resultados Finais
O projeto entrega uma solução "ponta-a-ponta" totalmente funcional:

Criação (YOLO): O script de Visão Computacional analisa um vídeo, deteta objetos (motos/carros) e envia os dados (moto_id, posicao_x) para a API .NET. A API salva esta nova moto no banco de dados PostgreSQL.

Atualização (IoT): O script de IoT (em paralelo) envia atualizações de status (status) para a API .NET. A API atualiza o registo da moto correspondente no banco.

Leitura (Dashboard): O index.html (dashboard) consulta a API .NET a cada 5 segundos, que lê o estado atual de todas as motos no banco e o retorna. O dashboard desenha visualmente a posição e o status (cor) de cada moto.

3. Instruções de Uso
Para executar o projeto completo, é necessário iniciar os 5 componentes em simultâneo. Siga este "Roteiro de Execução" de 6 passos.

Pré-requisitos
.NET 9 SDK

Docker Desktop

Python 3 (instalado pela Microsoft Store)

Ngrok (autenticado com um token de conta gratuita)

O Roteiro de Execução (5 Terminais)
Terminal 1️⃣: Banco de Dados (Docker)

Abra o Docker Desktop e espere que ele inicie (ícone 🐳 estável).

Num terminal (PowerShell/CMD), inicie o container do PostgreSQL:

Bash

docker start mottu-postgres
Terminal 2️⃣: API .NET (Servidor)

Num terminal, navegue até à pasta de "publicação" da API (o nosso código corrigido):

Bash

cd C:\Users\João\Downloads\Mottu-FrotaApi-main\Mottu-FrotaApi-main\Mottu-FrotaApi\bin\Debug\net9.0\publish
Execute a API (ela irá rodar na porta 5000):

Bash

dotnet Mottu.FrotaApi.dll
Deixe este terminal aberto. (Ele deve mostrar Now listening on: http://localhost:5000).

Terminal 3️⃣: Túnel (Ngrok)

Num terminal, navegue até à pasta do Ngrok:

Bash

E:
cd E:\ngrok
Inicie o túnel a apontar para a porta correta da API (5000):

Bash

.\ngrok.exe http 5000
O ngrok irá gerar um novo link https://.... Copie este link.

Deixe este terminal aberto.

Terminal 4️⃣: Dashboard (Frontend)

Num terminal, navegue até à pasta do seu dashboard:

Bash

E:
cd E:\Dashboard
Inicie o servidor web do Python (ele irá rodar na porta 8000):

Bash

python -m http.server 8000
Deixe este terminal aberto.

Componente 5️⃣: Simuladores (Google Colab)

Abra o seu Notebook Unificado (yolo_+_simulador_iot.py) no Google Colab.

Atualize os Links:

No Google Colab (Célula 2): Cole o novo link do Ngrok (do Passo 3) na variável NGROK_BASE_URL.

No seu index.html (na pasta E:\Dashboard): Abra-o no VS Code e cole o novo link do Ngrok na variável API_URL (linha 97), garantindo que termina com /api/patio/visualizar. (Salve o ficheiro após a alteração).

Carregue o Vídeo: Na barra de ficheiros à esquerda do Colab, faça o upload do video_teste_facil.mp4.

Execute a Célula 1 (!pip install...).

Execute a Célula 2 (Script Unificado).

Componente 6️⃣: Visualização

Abra o seu navegador e aceda a: http://localhost:8000

O pátio irá carregar e (após os 30s de atraso do IoT) começará a ser preenchido com as motos detectadas pelo YOLO e atualizadas pelo IoT.
