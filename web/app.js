const START_NODE_ID = 1;

const startScreen = document.getElementById("start-screen");
const gameScreen = document.getElementById("game-screen");
const startBtn = document.getElementById("start-btn");
const restartBtn = document.getElementById("restart-btn");
const storyText = document.getElementById("story-text");
const historyEl = document.getElementById("history");
const choicesEl = document.getElementById("choices");
const storyTreeEl = document.getElementById("story-tree");

let nodeMap = new Map();
let currentNodeId = START_NODE_ID;
let playerChoices = [];
let visitedNodeIds = new Set([START_NODE_ID]);
let treeRoots = [];

startBtn.addEventListener("click", () => {
  startScreen.classList.remove("active");
  gameScreen.classList.add("active");
  restartGame();
});

restartBtn.addEventListener("click", restartGame);

async function init() {
  const response = await fetch("story.json");
  if (!response.ok) {
    storyText.textContent = "Błąd: nie udało się załadować story.json.";
    return;
  }

  const data = await response.json();
  for (const node of data.nodes || []) {
    if (!nodeMap.has(node.id)) {
      nodeMap.set(node.id, node);
    }
  }

  treeRoots = buildTreePreview(START_NODE_ID, new Set(), 0, 4);
  renderTreePreview();
}

function restartGame() {
  currentNodeId = START_NODE_ID;
  playerChoices = [];
  visitedNodeIds = new Set([START_NODE_ID]);
  restartBtn.hidden = true;
  renderNode();
  renderTreePreview();
}

function renderNode() {
  const node = nodeMap.get(currentNodeId);
  if (!node) {
    storyText.textContent = `Błąd scenariusza: brak węzła o ID ${currentNodeId}.`;
    choicesEl.innerHTML = "";
    return;
  }

  storyText.textContent = node.text;
  renderHistory();
  renderTreePreview();

  if (node.isEnding) {
    choicesEl.innerHTML = "";
    restartBtn.hidden = false;
    return;
  }

  choicesEl.innerHTML = `
    <button class="choice-btn choice-btn-a" type="button" data-choice="A">
      <span class="label">A</span>
      <span>${escapeHtml(node.choiceAText)}</span>
    </button>
    <button class="choice-btn choice-btn-b" type="button" data-choice="B">
      <span class="label">B</span>
      <span>${escapeHtml(node.choiceBText)}</span>
    </button>
  `;

  choicesEl.querySelectorAll("[data-choice]").forEach((button) => {
    button.addEventListener("click", () => {
      const pick = button.dataset.choice;
      if (pick === "A") {
        playerChoices.push(`A: ${node.choiceAText}`);
        currentNodeId = node.choiceANodeId;
      } else {
        playerChoices.push(`B: ${node.choiceBText}`);
        currentNodeId = node.choiceBNodeId;
      }
      visitedNodeIds.add(currentNodeId);
      renderNode();
    });
  });
}

function renderHistory() {
  if (playerChoices.length === 0) {
    historyEl.className = "history empty";
    historyEl.textContent = "Ścieżka: brak wyborów.";
    return;
  }

  historyEl.className = "history";
  historyEl.innerHTML = "<strong>Ścieżka:</strong><br>" +
    playerChoices.map((choice) => `• ${escapeHtml(choice)}`).join("<br>");
}

function buildTreePreview(nodeId, seen, depth, maxDepth) {
  if (depth > maxDepth || seen.has(nodeId)) {
    return [];
  }

  const node = nodeMap.get(nodeId);
  if (!node) {
    return [];
  }

  const nextSeen = new Set(seen);
  nextSeen.add(nodeId);

  const entry = {
    id: node.id,
    depth,
    label: summarizeNode(node),
    isEnding: node.isEnding,
    children: [],
  };

  if (!node.isEnding) {
    entry.children = [
      ...buildTreePreview(node.choiceANodeId, nextSeen, depth + 1, maxDepth),
      ...buildTreePreview(node.choiceBNodeId, nextSeen, depth + 1, maxDepth),
    ];
  }

  return [entry];
}

function renderTreePreview() {
  const flat = flattenTree(treeRoots);
  storyTreeEl.innerHTML = flat.map((item) => {
    const classes = [
      "tree-node",
      `tree-depth-${Math.min(item.depth, 4)}`,
      visitedNodeIds.has(item.id) ? "visited" : "",
      item.id === currentNodeId ? "current" : "",
      item.isEnding ? "ending" : "",
    ].filter(Boolean).join(" ");

    return `<div class="${classes}">#${item.id} · ${escapeHtml(item.label)}</div>`;
  }).join("");
}

function flattenTree(entries) {
  const result = [];
  for (const entry of entries) {
    result.push(entry);
    result.push(...flattenTree(entry.children));
  }
  return result;
}

function summarizeNode(node) {
  const text = node.text.replace(/\s+/g, " ").trim();
  return text.length > 56 ? `${text.slice(0, 56)}…` : text;
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");
}

init();
