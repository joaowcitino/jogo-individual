# 🚀 Space Station Survivor

Um arcade shooter top-down 2D feito em Unity. Você é o piloto de uma nave presa em uma estação espacial em colapso: colete todos os cristais de energia antes que o oxigênio (O2) acabe, enquanto destrói e desvia de naves inimigas que patrulham e atiram.

**Autor:** João Citino
**Disciplina:** Jogos Digitais 2025.2 — Insper
**Engine:** Unity 6 (6000.3.12f1) · Universal Render Pipeline 2D
**Plataforma:** WebGL (900×600) — jogável no navegador

---

## 🎮 Como jogar

| Ação | Teclado | Controle Xbox |
|------|---------|---------------|
| Mover | `WASD` ou Setas | Analógico esquerdo |
| Atirar | `Espaço` | Gatilho direito (RT) |
| Reiniciar partida | `R` | — |

**Objetivo:** colete os **10 cristais** espalhados pela arena antes que o **O2 chegue a zero**.

**Cuidado:**
- Você tem **3 vidas**. Encostar em um inimigo ou levar um tiro custa 1 vida (com breve invencibilidade após o dano).
- A dificuldade aumenta com o tempo: aos **30s** os patrulheiros ficam agressivos e passam a te perseguir; aos **60s** os atiradores disparam mais rápido. Novas ondas de inimigos surgem a cada 20 segundos.

**Vitória:** coletar todos os 10 cristais.
**Derrota:** O2 zerar ou perder as 3 vidas.

---

## ✨ Funcionalidades

- **Sistema de tiro** na direção do movimento, com cooldown e rastro (trail) nos projéteis.
- **Dois tipos de inimigo:**
  - *Patrulheiro* — patrulha entre pontos e passa a perseguir o jogador com o tempo.
  - *Atirador* — reposiciona-se e dispara projéteis em direção ao jogador.
- **Ondas progressivas** de inimigos com escalonamento de dificuldade.
- **4 power-ups** que caem ao destruir inimigos:
  - 🟢 **O2** — +15 segundos de oxigênio
  - ⚡ **Tiro Rápido** — taxa de disparo triplicada por 5s
  - 🛡️ **Escudo** — absorve o próximo dano
  - 💨 **Velocidade** — movimento 2× mais rápido por 5s
- **Game feel:** screen shake ao tomar dano, partículas de explosão, HUD pulsante quando o O2 está baixo.
- **HUD completo:** O2, score, vidas (corações), contador de cristais e indicador de power-up ativo com barra de tempo.
- **Anti-softlock:** reinício a qualquer momento pela tecla `R` ou pelos botões da tela de fim de jogo; o cronômetro garante que a partida sempre termina.
- **Telas dedicadas:** Menu Principal, Jogo e Game Over (com fundos distintos para vitória e derrota).
- **Suporte a controle Xbox** via Unity Input System.

---

## 🗂️ Estrutura do projeto

```
Assets/
├── Scenes/        MainMenu · Game · GameOver
├── Scripts/
│   ├── Core/      GameManager, GameOverData, WaveManager
│   ├── Player/    PlayerController
│   ├── Enemy/     EnemyPatrol, EnemyShooter, EnemyHealth
│   ├── Combat/    Projectile
│   ├── Powerups/  PowerUp, PowerUpSpawner
│   ├── Audio/     AudioManager
│   ├── UI/        UIController, GameOverUI, MainMenuUI, PowerUpHUD
│   └── FX/        ScreenShake, FXManager
├── Sprites/       Kenney/ (sprites do jogo) · UI/ (telas)
├── Audio/         efeitos sonoros (.ogg)
└── Prefabs/       projéteis, inimigos e power-ups
```

---

## 🎨 Créditos dos Assets

Todos os assets utilizados são de uso livre. Agradecimentos aos autores:

### Sprites
- **Kenney — Space Shooter Redux**
  Naves, UFOs (inimigos), lasers, power-ups, estrelas (cristais) e backgrounds.
  Fonte: <https://kenney.nl/assets/space-shooter-redux>
  Licença: **CC0 1.0 (domínio público)** — <https://creativecommons.org/publicdomain/zero/1.0/>

### Efeitos sonoros
- **Kenney — Space Shooter Redux (Bonus pack)**
  Efeitos de laser, escudo e alarme (.ogg).
  Licença: **CC0 1.0 (domínio público)**

### Música
- **Música ambiente gerada proceduralmente em código** dentro do projeto (`AudioManager.cs`), sem arquivo externo.

### Imagens das telas (Menu, Vitória, Derrota)
- **Geradas por IA** (ChatGPT / DALL·E) com prompts autorais no estilo *synthwave*, exclusivamente para este projeto.

---

## 🛠️ Como rodar / compilar

1. Abra o projeto no **Unity 6 (6000.3.x)**.
2. Cena inicial: `Assets/Scenes/MainMenu.unity`.
3. Para a build de entrega: **File → Build Settings → WebGL**, resolução **900×600**, e *Build*.

---

## 🔗 Links

- **Jogue no navegador (itch.io):** _[adicionar link após o upload]_
- **Repositório:** _[este repositório]_

---

*Projeto individual desenvolvido para a disciplina de Jogos Digitais (Insper, 2025.2).*
