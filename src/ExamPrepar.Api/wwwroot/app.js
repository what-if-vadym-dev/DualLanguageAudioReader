// Render plain generated text into two columns
function renderGenerated(targetText, nativeText, prefs) {
  const targetLabel = document.getElementById('targetLabel');
  const nativeLabel = document.getElementById('nativeLabel');
  const targetCol = document.getElementById('target');
  const nativeCol = document.getElementById('native');
  targetLabel.textContent = langLabel(prefs.targetLanguage);
  nativeLabel.textContent = langLabel(prefs.nativeLanguage);
  targetCol.innerHTML = '';
  nativeCol.innerHTML = '';
  const tParas = (targetText || '').split(/\n\n+/);
  const nParas = (nativeText || '').split(/\n\n+/);
  tParas.forEach((p) => {
    const d = document.createElement('div');
    d.className = 'segment';
    d.innerHTML = '<div class="text"></div>';
    d.querySelector('.text').textContent = p;
    targetCol.appendChild(d);
  });
  nParas.forEach((p) => {
    const d = document.createElement('div');
    d.className = 'segment';
    d.innerHTML = '<div class="text"></div>';
    d.querySelector('.text').textContent = p;
    nativeCol.appendChild(d);
  });
}

// Preferences
function getPrefs() {
  try { return JSON.parse(localStorage.getItem('prefs') || '{}'); } catch { return {}; }
}
function setPrefs(p) { localStorage.setItem('prefs', JSON.stringify(p)); }
function langLabel(code) {
  const map = { 'nb-NO': 'Norwegian (Bokmål)', 'en-US': 'English (US)', 'es-ES': 'Spanish (Spain)', 'de-DE': 'German', 'fr-FR': 'French' };
  return map[code] || code || '';
}

// Audio + rendering
const player = document.getElementById('player');
const targetCol = document.getElementById('target');
const nativeCol = document.getElementById('native');
const playPauseBtn = document.getElementById('playPause');
const loadBtn = document.getElementById('loadAudio');
const audioUrlInput = document.getElementById('audioUrl');
const autoScroll = document.getElementById('autoScroll');
const targetLabel = document.getElementById('targetLabel');
const nativeLabel = document.getElementById('nativeLabel');
const openWizardBtn = document.getElementById('openWizardBtn');

let segments = [];
let activeIndex = -1;

function fmt(t) {
  const m = Math.floor(t / 60);
  const s = Math.floor(t % 60).toString().padStart(2, '0');
  return m + ':' + s;
}

function render() {
  targetCol.innerHTML = '';
  nativeCol.innerHTML = '';
  segments.forEach((seg, i) => {
    const tDiv = document.createElement('div');
    tDiv.className = 'segment';
    tDiv.dataset.index = i;
    tDiv.innerHTML = '<div class="time">' + fmt(seg.start) + ' – ' + fmt(seg.end) + '</div><div class="text">' + seg.target + '</div>';

    const nDiv = document.createElement('div');
    nDiv.className = 'segment';
    nDiv.dataset.index = i;
    nDiv.innerHTML = '<div class="time">' + fmt(seg.start) + ' – ' + fmt(seg.end) + '</div><div class="text">' + seg.native + '</div>';

    tDiv.addEventListener('click', () => { player.currentTime = seg.start; });
    nDiv.addEventListener('click', () => { player.currentTime = seg.start; });

    targetCol.appendChild(tDiv);
    nativeCol.appendChild(nDiv);
  });
}

function updateActive(idx) {
  if (activeIndex === idx) return;
  activeIndex = idx;
  document.querySelectorAll('.segment').forEach(el => {
    const i = Number(el.dataset.index);
    el.classList.toggle('active', i === idx);
    el.classList.toggle('past', i < idx);
  });
  if (autoScroll && autoScroll.checked && idx >= 0) {
    const tSeg = targetCol.querySelector('.segment[data-index="' + idx + '"]');
    const nSeg = nativeCol.querySelector('.segment[data-index="' + idx + '"]');
    if (tSeg) tSeg.scrollIntoView({ block: 'center' });
    if (nSeg) nSeg.scrollIntoView({ block: 'center' });
  }
}

player.addEventListener('timeupdate', () => {
  const ct = player.currentTime;
  const idx = segments.findIndex(s => ct >= s.start && ct < s.end);
  updateActive(idx);
});

playPauseBtn.addEventListener('click', () => {
  if (player.paused) { player.play(); playPauseBtn.textContent = 'Pause'; }
  else { player.pause(); playPauseBtn.textContent = 'Play'; }
});

loadBtn.addEventListener('click', () => {
  const url = audioUrlInput.value.trim();
  if (url) { player.src = url; player.load(); playPauseBtn.textContent = 'Play'; }
});

// Wizard
const wizard = document.getElementById('wizard');
const targetLangSel = document.getElementById('targetLang');
const nativeLangSel = document.getElementById('nativeLang');
const levelSel = document.getElementById('level');
const topicSel = document.getElementById('topic');
const wizardStart = document.getElementById('wizardStart');
const wizardClose = document.getElementById('wizardClose');

function showWizard() { wizard.hidden = false; }
function hideWizard() {
  if (!wizard || wizard.hidden) return;
  console.log('Hiding wizard overlay');
  wizard.hidden = true;
}
function fillWizard(prefs){
  try{ if(prefs.targetLanguage) targetLangSel.value = prefs.targetLanguage; }catch{}
  try{ if(prefs.nativeLanguage) nativeLangSel.value = prefs.nativeLanguage; }catch{}
  try{ if(prefs.level) levelSel.value = prefs.level; }catch{}
  try{ if(prefs.topic) topicSel.value = prefs.topic; }catch{}
}

async function fetchTranscripts(prefs) {
  try {
    const params = new URLSearchParams();
    if (prefs && prefs.targetLanguage) params.set('target', prefs.targetLanguage);
    if (prefs && prefs.nativeLanguage) params.set('native', prefs.nativeLanguage);
    if (prefs && prefs.level) params.set('level', prefs.level);
    const url = '/api/v1/transcripts' + (params.toString() ? ('?' + params.toString()) : '');
    const res = await fetch(url);
    if (!res.ok) return [];
    return await res.json();
  } catch { return []; }
}

async function loadTranscriptByPrefs(prefs) {
  targetLabel.textContent = langLabel(prefs.targetLanguage);
  nativeLabel.textContent = langLabel(prefs.nativeLanguage);
  const all = await fetchTranscripts(prefs);
  let chosenId = all && all.length ? all[0].id : null;
  try {
    const url = chosenId ? ('/api/v1/transcript?transcriptId=' + chosenId) : '/api/v1/transcript';
    const res = await fetch(url);
    if (res.ok) { segments = await res.json(); }
  } catch {}
  render();
}

wizardStart.addEventListener('click', async () => {
  const prefs = {
    targetLanguage: targetLangSel.value,
    nativeLanguage: nativeLangSel.value,
    level: levelSel.value,
    topic: topicSel.value,
  };
  console.log('Start Reading pressed', prefs);
  try {
    const res = await fetch('/api/v1/preferences', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(prefs)
    });
    if (res.ok) {
      const created = await res.json();
      console.log('Preferences saved on server', created);
      if (created && created.id) localStorage.setItem('preferenceId', created.id);
    }
  } catch (err) {
    console.error('Failed to persist preferences', err);
  }
  setPrefs(prefs);
  hideWizard();

  // Try AI generation first
  try {
    const gen = await fetch('/api/v1/generate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(prefs)
    });
    if (gen.ok) {
      const data = await gen.json();
      console.log('AI generation response', data);
      renderGenerated(data.targetText, data.nativeText, prefs);
      return;
    }
    console.warn('AI generation request failed', gen.status);
  } catch (err) {
    console.error('AI generation threw', err);
  }

  console.log('Falling back to stored transcripts', prefs);
  await loadTranscriptByPrefs(prefs);
});

if (openWizardBtn) {
  openWizardBtn.addEventListener('click', () => {
    const prefs = getPrefs();
    fillWizard(prefs || {});
    showWizard();
  });
}

// Init
(async function init() {
  const prefs = getPrefs();
  if (!prefs.targetLanguage || !prefs.nativeLanguage || !prefs.level || !prefs.topic) {
    fillWizard(prefs || {});
    showWizard();
  } else {
    try {
      const gen = await fetch('/api/v1/generate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(prefs)
      });
      if (gen.ok) {
        const data = await gen.json();
        renderGenerated(data.targetText, data.nativeText, prefs);
        return;
      }
    } catch {}
    await loadTranscriptByPrefs(prefs);
  }
})();


if (wizardClose) { wizardClose.addEventListener('click', () => hideWizard()); }
if (wizard) {
  wizard.addEventListener('click', (e) => {
    if (e.target === wizard) hideWizard();
  });
}
// Escape-to-close handler for the wizard overlay
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape' || e.key === 'Esc') {
    if (typeof wizard !== 'undefined' && wizard && !wizard.hidden) hideWizard();
  }
});
