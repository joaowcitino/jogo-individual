# 🚀 Space Station Survivor

▶️ **Jogue agora no navegador:** <https://joaowcitino.itch.io/space-station-survivor>

Um **arcade shooter top-down 2D** feito em Unity. Você pilota uma nave presa em uma estação espacial em colapso: colete cristais de energia para repor seu oxigênio e alcançar a meta, enquanto destrói e desvia de ondas de naves inimigas cada vez mais perigosas.

**Autor:** João Citino
**Disciplina:** Jogos Digitais 2025.2 — Insper
**Engine:** Unity 6 (6000.3.12f1) · Universal Render Pipeline 2D
**Plataforma:** WebGL (900×600) — jogável no navegador

---

## 🎮 Como jogar

| Ação | Teclado | Controle Xbox |
|------|---------|---------------|
| Mover | `WASD` ou Setas | Analógico esquerdo |
| Atirar (para cima) | `Espaço` | Gatilho direito (RT) / botão X |
| Reiniciar partida | `R` | — |

### Objetivo
Colete **50 cristais** para vencer. Cada cristal coletado **repõe oxigênio (+2,5s)** e dá **+50 de score** — ou seja, coletar não é só pontuar, é o que te mantém vivo. Os cristais **reaparecem** pelo mapa conforme você os pega, então sempre há o que caçar.

### Cuidado
- Você tem **3 vidas**. Encostar em um inimigo ou levar um tiro custa 1 vida (com breve invencibilidade após o dano).
- O **oxigênio (O2) drena com o tempo** — se chegar a zero, é game over. Continue coletando cristais para sobreviver.
- A **dificuldade aumenta progressivamente**: as ondas de inimigos ficam mais frequentes, em maior número, e a proporção de naves atiradoras cresce. Patrulheiros passam a perseguir você com o tempo.

### Como ganhar / perder
- **Vitória:** coletar 50 cristais.
- **Derrota:** O2 chegar a zero **ou** perder as 3 vidas.

---

## ✨ Funcionalidades

- **Sistema de tiro** controlado: dispara reto para cima apenas ao apertar o botão (com cadência/cooldown e rastro nos projéteis).
- **Cristais que reaparecem** mantendo sempre 8 no mapa — fonte contínua de O2 e pontos (loop de coleta dinâmico).
- **Dois tipos de inimigo:**
  - *Patrulheiro* — patrulha o mapa e passa a perseguir o jogador conforme o tempo avança.
  - *Atirador* — reposiciona-se e dispara projéteis em direção ao jogador (mais rápido com o tempo).
- **Ondas progressivas** com escalonamento de dificuldade (intervalo, quantidade e proporção de atiradores escalam com o tempo de jogo).
- **4 power-ups** que caem das naves destruídas, com sprites próprios + animação (pulsam e giram):
  - 🟢 **O2** — +15 segundos de oxigênio
  - ⚡ **Tiro Rápido** — cadência de disparo triplicada por 5s
  - 🛡️ **Escudo** — absorve o próximo dano
  - 💨 **Velocidade** — movimento 2× mais rápido por 5s
- **Game feel:** screen shake ao tomar dano, partículas de explosão, escudo visual, HUD que pulsa em vermelho quando o O2 está baixo.
- **HUD completo:** O2, score, vidas (corações), contador de cristais (X/50) e indicador de power-up ativo com barra de tempo.
- **Anti-softlock:** reinício a qualquer momento (`R` ou botões da tela de fim de jogo); o cronômetro garante que a partida sempre termina.
- **Telas dedicadas** com arte synthwave: Menu Principal, Jogo, Game Over (fundos distintos para vitória e derrota).
- **Suporte a controle Xbox** via Unity Input System.

---

## 🗂️ Estrutura do projeto

```
Assets/
├── Scenes/        MainMenu · Game · GameOver
├── Scripts/
│   ├── Core/      GameManager · GameOverData · WaveManager · CrystalSpawner
│   ├── Player/    PlayerController
│   ├── Enemy/     EnemyPatrol · EnemyShooter · EnemyHealth
│   ├── Combat/    Projectile
│   ├── Powerups/  PowerUp · PowerUpSpawner · PowerUpVisual
│   ├── Audio/     AudioManager
│   ├── UI/        UIController · GameOverUI · MainMenuUI · PowerUpHUD
│   └── FX/        ScreenShake · FXManager
├── Sprites/       Kenney/ (sprites do jogo) · UI/ (telas synthwave)
├── Audio/         efeitos sonoros (.ogg)
└── Prefabs/       projéteis, inimigos, power-ups e cristal
```

---

## 🎨 Créditos dos Assets

Todos os assets utilizados são de uso livre. Agradecimentos aos autores:

### Sprites e efeitos sonoros
- **Kenney — Space Shooter Redux**
  Naves, UFOs (inimigos), lasers, power-ups, estrelas (cristais), backgrounds e efeitos sonoros (.ogg).
  Fonte: <https://kenney.nl/assets/space-shooter-redux>
  Licença: **CC0 1.0 (domínio público)** — <https://creativecommons.org/publicdomain/zero/1.0/>

### Música
- **Música ambiente gerada proceduralmente em código** dentro do projeto (`AudioManager.cs`), sem arquivo externo.

### Imagens das telas (Menu, Vitória, Derrota)
- **Geradas por IA** (ChatGPT / DALL·E) com prompts autorais no estilo *synthwave*, exclusivamente para este projeto.

> O design de balanceamento (meta de 50 cristais com respawn, cristais como fonte de O2, escalonamento de ondas) foi inspirado em padrões de jogos arcade clássicos como Asteroids, Geometry Wars, Galaga e jogos do gênero *survivor-like*.

---

## 🛠️ Como rodar / compilar

1. Abra o projeto no **Unity 6 (6000.3.x)**.
2. Cena inicial: `Assets/Scenes/MainMenu.unity`.
3. Build de entrega: **File → Build Settings → WebGL**, resolução **900×600**, e *Build*.

---

## 🔗 Links

- **Jogue no navegador (itch.io):** <https://joaowcitino.itch.io/space-station-survivor>
- **Repositório:** <https://github.com/joaowcitino/jogo-individual>

---

*Projeto individual desenvolvido para a disciplina de Jogos Digitais (Insper, 2025.2).*
