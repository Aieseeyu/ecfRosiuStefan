// ================== CONFIG ==================
const root = document.getElementById("appRoot");
const api = "https://localhost:7061/api"; // adapte si besoin

// ================== HELPERS ==================
const $ = (s) => document.querySelector(s);
const $$ = (s) => document.querySelectorAll(s);

async function req(method, url, data) {
  const opt = { method, headers: { "Content-Type": "application/json" } };
  if (data) opt.body = JSON.stringify(data);
  const res = await fetch(url, opt);
  if (!res.ok) throw new Error((await res.text()) || res.statusText);
  return res.status === 204 ? null : res.json();
}

// ================== ROUTING ==================
async function load(page) {
  const r = await fetch(`./pages/${page}.html`);
  root.innerHTML = await r.text();
  $$(".nav-link").forEach((a) => a.classList.remove("active"));
  $(`.nav-link[data-page="${page}"]`)?.classList.add("active");
  if (page === "lecons") initLecons();
  if (page === "eleves") initEleves();
  if (page === "moniteurs") initMoniteurs();
  if (page === "vehicules") initVehicules();
  if (page === "modeles") initModeles();
  if (page === "calendriers") initCalendriers();
}

$$("[data-page]").forEach((a) =>
  a.addEventListener("click", (e) => {
    e.preventDefault();
    load(a.dataset.page);
  })
);
load("lecons");

// ================== PAGE LEÇONS ==================
async function initLecons() {
  const list = $("#listLecons");
  const form = $("#formLecon");
  const msg = $("#msg");

  const sModele = $("#s_modele");
  const sDate = $("#s_date");
  const sEleve = $("#s_eleve");
  const sMoniteur = $("#s_moniteur");
  const fDuree = $("#f_duree");

  // --- charge les options des selects ---
  async function fillSelects() {
    // Modeles
    const modeles = await req("GET", `${api}/modeles`);
    sModele.innerHTML = modeles
      .map(
        (m) =>
          `<option value="${escapeHtml(m.modeleVehicule)}">${escapeHtml(
            m.modeleVehicule
          )} — ${escapeHtml(m.marque)}</option>`
      )
      .join("");

    // Dates (Calendrier) — tri décroissant (récent d'abord)
    const dates = await req("GET", `${api}/calendriers`);
    const items = (dates || [])
      .map((c) => (c.dateHeure || "").substring(0, 10)) // "YYYY-MM-DD"
      .filter(Boolean)
      .sort()
      .reverse();

    sDate.innerHTML = items
      .map((d) => `<option value="${d}">${d}</option>`)
      .join("");

    // sélection par défaut: la plus récente si présente
    sDate.value = items[0] || "";

    // Eleves
    const eleves = await req("GET", `${api}/eleves`);
    sEleve.innerHTML = eleves
      .map(
        (e) =>
          `<option value="${e.idEleve}">${e.idEleve} — ${escapeHtml(
            e.nomEleve
          )} ${escapeHtml(e.prenomEleve)}</option>`
      )
      .join("");

    // Moniteurs
    const monos = await req("GET", `${api}/moniteurs`);
    sMoniteur.innerHTML = monos
      .map(
        (m) =>
          `<option value="${m.idMoniteur}">${m.idMoniteur} — ${escapeHtml(
            m.nomMoniteur
          )} ${escapeHtml(m.prenomMoniteur)} ${
            m.activite ? "" : "(inactif)"
          }</option>`
      )
      .join("");

    // valeur par défaut
    fDuree.value = 60;
  }

  // --- liste des leçons ---
  async function loadLecons() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/lecons`);
      if (!data.length) {
        list.innerHTML = `<div class="text-center text-muted py-3">Aucune leçon.</div>`;
        return;
      }
      list.innerHTML = "";
      data.forEach((l) => list.appendChild(row(l)));
    } catch (e) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${e.message}</div>`;
    }
  }

  function row(l) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.modele = l.modeleVehicule;
    d.dataset.date = l.dateHeure.substring(0, 10);
    d.dataset.eleve = l.idEleve;
    d.dataset.moniteur = l.idMoniteur;

    d.innerHTML = `
      <div class="col-8">
        <strong>${escapeHtml(
          l.modeleVehicule
        )}</strong> — ${l.dateHeure.substring(0, 10)}
        — élève ${l.idEleve}, moniteur ${l.idMoniteur} — ${l.duree} min
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-edit me-2">Modifier</button>
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    // Modifier -> remplit le formulaire
    d.querySelector(".btn-edit").onclick = () => {
      sModele.value = l.modeleVehicule;
      sDate.value = l.dateHeure.substring(0, 10);
      sEleve.value = String(l.idEleve);
      sMoniteur.value = String(l.idMoniteur);
      fDuree.value = l.duree;
      msg.textContent = "Leçon chargée pour modification.";
      window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    };

    // Supprimer juste cette ligne
    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req(
          "DELETE",
          `${api}/lecons/${encodeURIComponent(d.dataset.modele)}/${
            d.dataset.date
          }/${d.dataset.eleve}/${d.dataset.moniteur}`
        );
        d.remove();
        msg.textContent = "Leçon supprimée.";
      } catch (e) {
        msg.textContent = "Erreur suppression : " + e.message;
      }
    };

    return d;
  }

  // --- save (create or update) ---
  async function save(e) {
    e.preventDefault();
    const payload = {
      modeleVehicule: sModele.value,
      dateHeure: sDate.value,
      idEleve: parseInt(sEleve.value, 10),
      idMoniteur: parseInt(sMoniteur.value, 10),
      duree: parseInt(fDuree.value, 10),
    };

    // on tente update d'abord, puis création si 404
    try {
      await req(
        "PUT",
        `${api}/lecons/${encodeURIComponent(payload.modeleVehicule)}/${
          payload.dateHeure
        }/${payload.idEleve}/${payload.idMoniteur}`,
        payload
      );
      msg.textContent = "Leçon mise à jour.";
    } catch (_) {
      try {
        await req("POST", `${api}/lecons`, payload);
        msg.textContent = "Leçon créée.";
      } catch (e2) {
        msg.textContent = "Erreur : " + e2.message;
        return;
      }
    }
    await loadLecons();
  }

  // --- outils ---
  function escapeHtml(s) {
    return String(s).replace(
      /[&<>"']/g,
      (m) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        }[m])
    );
  }

  // bind
  form.addEventListener("submit", save);
  $("#btnReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Nouveau formulaire.";
  });

  // start
  await fillSelects();
  await loadLecons();
}

async function initEleves() {
  const list = document.getElementById("listEleves");
  const form = document.getElementById("formEleve");
  const msg = document.getElementById("msgEleve");

  const eId = document.getElementById("e_id");
  const eNom = document.getElementById("e_nom");
  const ePren = document.getElementById("e_prenom");
  const eCode = document.getElementById("e_code");
  const eCond = document.getElementById("e_conduite");
  const eNaiss = document.getElementById("e_naiss");

  function esc(s) {
    return String(s).replace(
      /[&<>"']/g,
      (m) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        }[m])
    );
  }

  // Liste
  async function loadEleves() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/eleves`);
      if (!data.length) {
        list.innerHTML = `<div class="text-center text-muted py-3">Aucun élève.</div>`;
        return;
      }
      list.innerHTML = "";
      data.forEach((e) => list.appendChild(row(e)));
    } catch (err) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${err.message}</div>`;
    }
  }

  // Une ligne
  function row(e) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.id = e.idEleve;

    const dateStr = (e.dateNaissance || "").substring(0, 10);

    d.innerHTML = `
      <div class="col-8">
        <strong>#${e.idEleve}</strong> — ${esc(e.nomEleve)} ${esc(
      e.prenomEleve
    )}
        — Code: ${e.code ? "✅" : "❌"} — Conduite: ${e.conduite ? "✅" : "❌"}
        — Né(e): ${dateStr || "-"}
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-edit me-2">Modifier</button>
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    // Modifier -> remplit le formulaire
    d.querySelector(".btn-edit").onclick = () => {
      eId.value = e.idEleve;
      eNom.value = e.nomEleve;
      ePren.value = e.prenomEleve;
      eCode.checked = !!e.code;
      eCond.checked = !!e.conduite;
      eNaiss.value = dateStr || "";
      msg.textContent = "Élève chargé pour modification.";
      window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    };

    // Supprimer
    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req("DELETE", `${api}/eleves/${d.dataset.id}`);
        d.remove();
        msg.textContent = "Élève supprimé.";
      } catch (er) {
        msg.textContent = "Erreur suppression : " + er.message;
      }
    };

    return d;
  }

  // Save (PUT si existe, sinon POST)
  async function saveEleve(ev) {
    ev.preventDefault();
    const payload = {
      idEleve: parseInt(eId.value, 10),
      nomEleve: eNom.value.trim(),
      prenomEleve: ePren.value.trim(),
      code: !!eCode.checked,
      conduite: !!eCond.checked,
      dateNaissance: eNaiss.value,
    };

    if (!payload.idEleve) {
      msg.textContent = "ID requis.";
      return;
    }

    // on tente PUT, si échec -> POST
    try {
      await req("PUT", `${api}/eleves/${payload.idEleve}`, payload);
      msg.textContent = "Élève mis à jour.";
    } catch {
      try {
        await req("POST", `${api}/eleves`, payload);
        msg.textContent = "Élève créé.";
      } catch (er2) {
        msg.textContent = "Erreur : " + er2.message;
        return;
      }
    }
    await loadEleves();
  }

  // Bind
  form.addEventListener("submit", saveEleve);
  document.getElementById("btnEleveReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Nouveau formulaire.";
  });

  // Start
  await loadEleves();
}

async function initMoniteurs() {
  const list = document.getElementById("listMoniteurs");
  const form = document.getElementById("formMoniteur");
  const msg = document.getElementById("msgMoniteur");

  const mId = document.getElementById("m_id");
  const mNom = document.getElementById("m_nom");
  const mPre = document.getElementById("m_prenom");
  const mNais = document.getElementById("m_naiss");
  const mEmb = document.getElementById("m_embauche");
  const mAct = document.getElementById("m_actif");

  const esc = (s) =>
    String(s).replace(
      /[&<>"']/g,
      (c) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        }[c])
    );

  // Liste
  async function loadMoniteurs() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/moniteurs`);
      if (!data.length) {
        list.innerHTML = `<div class="text-center text-muted py-3">Aucun moniteur.</div>`;
        return;
      }
      list.innerHTML = "";
      data.forEach((m) => list.appendChild(row(m)));
    } catch (err) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${err.message}</div>`;
    }
  }

  // Une ligne
  function row(m) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.id = m.idMoniteur;

    const n = (m.dateNaissance || "").substring(0, 10);
    const e = (m.dateEmbauche || "").substring(0, 10);

    d.innerHTML = `
      <div class="col-8">
        <strong>#${m.idMoniteur}</strong> — ${esc(m.nomMoniteur)} ${esc(
      m.prenomMoniteur
    )}
        — Né(e): ${n || "-"} — Embauche: ${e || "-"} — Actif: ${
      m.activite ? "✅" : "❌"
    }
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-edit me-2">Modifier</button>
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    // Modifier -> remplit le formulaire
    d.querySelector(".btn-edit").onclick = () => {
      mId.value = m.idMoniteur;
      mNom.value = m.nomMoniteur;
      mPre.value = m.prenomMoniteur;
      mNais.value = n || "";
      mEmb.value = e || "";
      mAct.checked = !!m.activite;
      msg.textContent = "Moniteur chargé pour modification.";
      window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    };

    // Supprimer
    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req("DELETE", `${api}/moniteurs/${d.dataset.id}`);
        d.remove();
        msg.textContent = "Moniteur supprimé.";
      } catch (er) {
        msg.textContent = "Erreur suppression : " + er.message;
      }
    };

    return d;
  }

  // Save (PUT si existe, sinon POST)
  async function saveMoniteur(ev) {
    ev.preventDefault();
    const payload = {
      idMoniteur: parseInt(mId.value, 10),
      nomMoniteur: mNom.value.trim(),
      prenomMoniteur: mPre.value.trim(),
      dateNaissance: mNais.value,
      dateEmbauche: mEmb.value,
      activite: !!mAct.checked,
    };

    if (!payload.idMoniteur) {
      msg.textContent = "ID requis.";
      return;
    }

    try {
      await req("PUT", `${api}/moniteurs/${payload.idMoniteur}`, payload);
      msg.textContent = "Moniteur mis à jour.";
    } catch {
      try {
        await req("POST", `${api}/moniteurs`, payload);
        msg.textContent = "Moniteur créé.";
      } catch (er2) {
        msg.textContent = "Erreur : " + er2.message;
        return;
      }
    }
    await loadMoniteurs();
  }

  // Bind
  form.addEventListener("submit", saveMoniteur);
  document.getElementById("btnMoniteurReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Nouveau formulaire.";
  });

  // Start
  await loadMoniteurs();
}

async function initVehicules() {
  const list = document.getElementById("listVehicules");
  const form = document.getElementById("formVehicule");
  const msg = document.getElementById("msgVeh");

  const vIm = document.getElementById("v_immat");
  const vMo = document.getElementById("v_modele");
  const vEt = document.getElementById("v_etat");

  const esc = (s) =>
    String(s).replace(
      /[&<>"']/g,
      (c) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        }[c])
    );

  // Remplir la liste des modèles
  async function fillModeles() {
    try {
      const modeles = await req("GET", `${api}/modeles`);
      vMo.innerHTML = modeles
        .map(
          (m) =>
            `<option value="${esc(m.modeleVehicule)}">${esc(
              m.modeleVehicule
            )} — ${esc(m.marque)}</option>`
        )
        .join("");
    } catch (e) {
      vMo.innerHTML = `<option value="">(erreur chargement modèles)</option>`;
      msg.textContent = "Erreur modèles : " + e.message;
    }
  }

  // Liste des véhicules
  async function loadVehicules() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/vehicules`);
      if (!data.length) {
        list.innerHTML = `<div class="text-center text-muted py-3">Aucun véhicule.</div>`;
        return;
      }
      list.innerHTML = "";
      data.forEach((v) => list.appendChild(row(v)));
    } catch (err) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${err.message}</div>`;
    }
  }

  // Une ligne
  function row(v) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.immat = v.immatriculation;

    d.innerHTML = `
      <div class="col-8">
        <strong>${esc(v.immatriculation)}</strong> — Modèle: ${esc(
      v.modeleVehicule
    )} — État: ${v.etat ? "✅" : "❌"}
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-edit me-2">Modifier</button>
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    // Modifier -> remplit le formulaire
    d.querySelector(".btn-edit").onclick = () => {
      vIm.value = v.immatriculation;
      vMo.value = v.modeleVehicule;
      vEt.checked = !!v.etat;
      msg.textContent = "Véhicule chargé pour modification.";
      window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    };

    // Supprimer
    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req(
          "DELETE",
          `${api}/vehicules/${encodeURIComponent(d.dataset.immat)}`
        );
        d.remove();
        msg.textContent = "Véhicule supprimé.";
      } catch (er) {
        msg.textContent = "Erreur suppression : " + er.message;
      }
    };

    return d;
  }

  // Save (PUT si existe, sinon POST)
  async function saveVehicule(ev) {
    ev.preventDefault();
    const payload = {
      immatriculation: vIm.value.trim(),
      modeleVehicule: vMo.value,
      etat: !!vEt.checked,
    };
    if (!payload.immatriculation) {
      msg.textContent = "Immatriculation requise.";
      return;
    }

    try {
      await req(
        "PUT",
        `${api}/vehicules/${encodeURIComponent(payload.immatriculation)}`,
        payload
      );
      msg.textContent = "Véhicule mis à jour.";
    } catch {
      try {
        await req("POST", `${api}/vehicules`, payload);
        msg.textContent = "Véhicule créé.";
      } catch (er2) {
        msg.textContent = "Erreur : " + er2.message;
        return;
      }
    }
    await loadVehicules();
  }

  // Bind
  form.addEventListener("submit", saveVehicule);
  document.getElementById("btnVehReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Nouveau formulaire.";
  });

  // Go
  await fillModeles();
  await loadVehicules();
}

async function initModeles() {
  const list = document.getElementById("listModeles");
  const form = document.getElementById("formModele");
  const msg = document.getElementById("msgModele");

  const mMod = document.getElementById("mo_modele");
  const mMar = document.getElementById("mo_marque");
  const mAnn = document.getElementById("mo_annee");
  const mAch = document.getElementById("mo_achat");

  const esc = (s) =>
    String(s).replace(
      /[&<>"']/g,
      (c) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        }[c])
    );

  // Liste
  async function loadModeles() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/modeles`);
      if (!data.length) {
        list.innerHTML = `<div class="text-center text-muted py-3">Aucun modèle.</div>`;
        return;
      }
      list.innerHTML = "";
      data.forEach((m) => list.appendChild(row(m)));
    } catch (err) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${err.message}</div>`;
    }
  }

  // Une ligne
  function row(m) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.modele = m.modeleVehicule;

    const achat = (m.dateAchat || "").substring(0, 10);

    d.innerHTML = `
      <div class="col-8">
        <strong>${esc(m.modeleVehicule)}</strong> — ${esc(m.marque)} — ${esc(
      m.annee
    )} — Achat: ${achat || "-"}
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-edit me-2">Modifier</button>
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    // Modifier -> remplit le formulaire
    d.querySelector(".btn-edit").onclick = () => {
      mMod.value = m.modeleVehicule;
      mMar.value = m.marque;
      mAnn.value = m.annee;
      mAch.value = achat || "";
      msg.textContent = "Modèle chargé pour modification.";
      window.scrollTo({ top: document.body.scrollHeight, behavior: "smooth" });
    };

    // Supprimer
    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req(
          "DELETE",
          `${api}/modeles/${encodeURIComponent(d.dataset.modele)}`
        );
        d.remove();
        msg.textContent = "Modèle supprimé.";
      } catch (er) {
        msg.textContent = "Erreur suppression : " + er.message;
      }
    };

    return d;
  }

  // Save (PUT si existe, sinon POST)
  async function saveModele(ev) {
    ev.preventDefault();

    const modele = mMod.value.trim();
    const marque = mMar.value.trim();
    const annee = mAnn.value.trim();
    const achat = mAch.value;

    // validations simples côté front
    if (!modele) {
      msg.textContent = "Modèle requis.";
      return;
    }
    if (!marque) {
      msg.textContent = "Marque requise.";
      return;
    }
    if (!/^\d{4}$/.test(annee)) {
      msg.textContent = "Année invalide (AAAA).";
      return;
    }
    if (!achat) {
      msg.textContent = "Date d'achat requise.";
      return;
    }

    const payload = { modeleVehicule: modele, marque, annee, dateAchat: achat };

    try {
      await req("PUT", `${api}/modeles/${encodeURIComponent(modele)}`, payload);
      msg.textContent = "Modèle mis à jour.";
    } catch {
      try {
        await req("POST", `${api}/modeles`, payload);
        msg.textContent = "Modèle créé.";
      } catch (er2) {
        msg.textContent = "Erreur : " + er2.message;
        return;
      }
    }
    await loadModeles();
  }

  // Bind
  form.addEventListener("submit", saveModele);
  document.getElementById("btnModeleReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Nouveau formulaire.";
  });

  // Start
  await loadModeles();
}

async function initCalendriers() {
  const list = document.getElementById("listCalendriers");
  const form = document.getElementById("formCalendrier");
  const msg = document.getElementById("msgCal");
  const cDate = document.getElementById("c_date");

  const selMonth = document.getElementById("c_month");
  const selYear = document.getElementById("c_year");
  const btnReset = document.getElementById("c_reset");

  let allDates = []; // cache des dates venues de l'API

  // --- Chargement initial ---
  async function loadCalendriers() {
    list.innerHTML = `<div class="text-center text-muted py-3">Chargement...</div>`;
    try {
      const data = await req("GET", `${api}/calendriers`);
      // normalise + tri
      allDates = (data || [])
        .map((x) => ({ dateStr: (x.dateHeure || "").substring(0, 10) }))
        .filter((x) => x.dateStr);
      allDates.sort((a, b) => a.dateStr.localeCompare(b.dateStr));

      buildYearOptions();
      render();
    } catch (err) {
      list.innerHTML = `<div class="text-danger text-center py-3">Erreur : ${err.message}</div>`;
    }
  }

  // --- Construit la liste des années selon les données ---
  function buildYearOptions() {
    const years = Array.from(
      new Set(allDates.map((d) => d.dateStr.slice(0, 4)))
    ).sort();
    const current = selYear.value;
    selYear.innerHTML =
      `<option value="">Toutes les années</option>` +
      years.map((y) => `<option value="${y}">${y}</option>`).join("");
    // restaure la sélection si possible
    if (years.includes(current)) selYear.value = current;
  }

  // --- Rendu avec filtre ---
  function render() {
    const m = parseInt(selMonth.value || "", 10); // NaN si vide
    const y = selYear.value; // "" si vide

    const filtered = allDates.filter((d) => {
      const year = d.dateStr.slice(0, 4);
      const month = parseInt(d.dateStr.slice(5, 7), 10);
      const okY = y ? year === y : true;
      const okM = m ? month === m : true;
      return okY && okM;
    });

    if (!filtered.length) {
      list.innerHTML = `<div class="text-center text-muted py-3">Aucune date.</div>`;
      return;
    }

    list.innerHTML = "";
    filtered.forEach((c) => list.appendChild(row(c.dateStr)));
  }

  // --- Une ligne ---
  function row(dateStr) {
    const d = document.createElement("div");
    d.className = "row align-items-center m-0 p-3 border-bottom";
    d.dataset.date = dateStr;

    d.innerHTML = `
      <div class="col-8">
        <strong>${dateStr}</strong>
      </div>
      <div class="col-4 text-end">
        <button class="btn btn-sm btn-danger">Supprimer</button>
      </div>
    `;

    d.querySelector(".btn-danger").onclick = async () => {
      try {
        await req("DELETE", `${api}/calendriers/${d.dataset.date}`);
        // retire aussi du cache puis re-render
        allDates = allDates.filter((x) => x.dateStr !== d.dataset.date);
        buildYearOptions();
        render();
        msg.textContent = "Date supprimée.";
      } catch (er) {
        msg.textContent = "Erreur suppression : " + er.message;
      }
    };

    return d;
  }

  // --- Ajout d'une date ---
  async function addDate(ev) {
    ev.preventDefault();
    const val = cDate.value;
    if (!val) {
      msg.textContent = "Veuillez choisir une date.";
      return;
    }

    try {
      await req("POST", `${api}/calendriers`, { dateHeure: val });
      // ajoute au cache si pas déjà présent
      if (!allDates.some((x) => x.dateStr === val)) {
        allDates.push({ dateStr: val });
        allDates.sort((a, b) => a.dateStr.localeCompare(b.dateStr));
        buildYearOptions();
      }
      msg.textContent = "Date ajoutée.";
      form.reset();
      render(); // re-filtre selon mois/année courants
    } catch (e2) {
      msg.textContent = "Erreur : " + e2.message;
    }
  }

  // --- Écouteurs filtres ---
  selMonth.addEventListener("change", render);
  selYear.addEventListener("change", render);
  btnReset.addEventListener("click", () => {
    selMonth.value = "";
    selYear.value = "";
    render();
  });

  // --- Écouteurs formulaire ---
  form.addEventListener("submit", addDate);
  document.getElementById("btnCalReset").addEventListener("click", () => {
    form.reset();
    msg.textContent = "Formulaire réinitialisé.";
  });

  // --- GO ---
  await loadCalendriers();
}
