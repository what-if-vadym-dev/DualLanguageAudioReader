You are a principal .NET Core architect and engineer.
Your task is to design a .NET Core application that serves as a platform for preparing users for language exams.
The platform must:

UI 

implement ui, there should be two column with text, target language to learn and your mother language, once it is played, both text have make down of the reading part.

Session Prompts Log (2026-02-18)
- .NET 8 solution skeleton with projects and folder structure
- run solution
- implement ui, there should be two column with text, target language to learn and your mother language
- connect ui with the backend
- update prompts file with the changes to ui
- mark completed tasks in the prompts.md file
- add page wizard: what language to learn, what is your mother language, what is the level
- update level to have a1 a2 b1 b2 c1 levels
- connect language wizard with backend, persist to database
- add topics dropdown with static list; note dynamic generation by country/level in prompts
- once start reading pressed, dialog is hidden and main screen is shown; add button "Set Up Your Reading"
- on start reading, save settings to backend and hide dialog
- add cross on the dialog "Set Up Your Reading" to close it
- add function to close dialog when you click cross
- esc to close
- close function is not called on click reading/cross/esc
- hide the window on "Start Reading" and add logs to console
- push to https://github.com/what-if-vadym-dev/DualLanguageAudioReader
- create docker image and run it in docker desktop
- update README.md with docker run instructions
- create basic ci/cd with test and prod environments
- deploy to Azure container host with minimum power consumption
- add infrastructure pipeline with provision using the given values
- add README steps for AZURE_CREDENTIALS secret
- stop in visual studio code
- how to open it in the browser

Generate personalized tests tailored to different proficiency levels and multiple languages.
Provide machine‑generated text‑to‑speech (TTS) audio for the free version.
For the paid version, allow users to:

Pay for professionally voiced audio recorded by native speakers.
Upload text and receive custom native‑speaker voiceovers, provided by local voice actors, for an additional fee.



Technical Task Breakdown (Backlog)
Working Assumptions (Tech Baseline)

Backend: .NET 8, ASP.NET Core Minimal APIs, C# 12, MediatR for app layer, EF Core for data.
Frontend: Blazor (WASM or Server) or React + TypeScript (choose later; tasks list both).
DB: PostgreSQL (prefer Azure Database for PostgreSQL Flexible Server), Redis for cache.
Storage: Azure Blob Storage, CDN fronted.
Queues/Events: Azure Service Bus.
Auth: OpenID Connect (Azure AD B2C or Auth0), OAuth for social.
TTS: Azure Cognitive Services (Speech).
Payments: Stripe (subscriptions + payouts), SEPA; VAT handling for EU/NO.
Observability: OpenTelemetry, Azure Monitor.
Region: Azure Norway East (primary).


You can tune stack choices later; the task structure remains valid.


Epic A: Identity & Access (AUTH)
Outcome: Secure auth with roles: Learner, Educator, Voice Actor, Content Editor, Admin. Org memberships supported.
A‑US‑001: As a user, I can register and login with email/password and OAuth
Acceptance Criteria (G/W/T)

Given I submit valid credentials → When I call /auth/login → Then I receive access/refresh tokens (JWT) with role claims.
Given I login via Microsoft/Google/Apple → Then a linked account is created (if first-time) and I am signed in.

Technical Tasks

AUTH‑101: Stand up /auth endpoints: register, login, refresh, logout, password/reset (ASP.NET Core Identity or custom + EF Core). 8 SP
AUTH‑102: OAuth/OIDC integration (Microsoft, Google, Apple); configure redirect URIs; handle linked accounts. 8 SP
AUTH‑103: JWT issuing & validation; refresh token rotation; revoke on logout. 5 SP
AUTH‑104: Role & policy-based authorization; implement roles and permissions matrix. 5 SP
AUTH‑105: MFA (TOTP/email link) toggle; backup codes. 8 SP (Phase 2 optional)
AUTH‑106: Security hardening: lockout policy, password strength, CSRF, CORS, device/session mgmt. 5 SP
AUTH‑107: Org/Membership model; Organization, Membership entities & CRUD. 8 SP
AUTH‑108: Audit trail (user creation, role changes). 3 SP

Dependencies: Core DB migrations (PLT‑101).
Test: E2E login via OAuth & email; invalid token rejection; refresh rotation.

Epic B: Profiles & Onboarding (PROF)
Outcome: User preferences, languages, goals, baseline placement.
B‑US‑001: As a learner, I complete onboarding and take a placement test
Acceptance Criteria

Given I complete onboarding → I can pick target language(s), exam(s), goals.
Placement test returns an estimated CEFR level (A1–C2).

Technical Tasks

PROF‑101: Profile schema; preferences (locale, accent, speed), privacy & consents. 5 SP
PROF‑102: Onboarding UI flow (steps: goals → language → exam → placement). 8 SP
PROF‑103: Placement test stub (calls Assessment service). 3 SP
PROF‑104: Localization resources (i18n), nb-NO & en-US to start. 5 SP
PROF‑105: Data privacy settings UI (export data, delete account links). 5 SP

Dependencies: Assessment baseline (ASS‑101...ASS‑107).

Epic C: Content Management (CMS)
Outcome: Versioned question bank, passages, assets, with editorial workflow.
C‑US‑001: As an editor, I can create and manage items/passages with tagging and approvals
Acceptance Criteria

CRUD items/passages; tags: language, skill, topic, difficulty, metadata.
Versioning; approvals required before exposure to learners.

Technical Tasks

CMS‑101: Entities: Item, Passage, Asset, Tag, License; EF Core migrations. 8 SP
CMS‑102: Item CRUD APIs; validation & schemas; server-side normalization (text, diacritics). 8 SP
CMS‑103: Versioning & approvals workflow (draft → review → approved). 8 SP
CMS‑104: Bulk import CSV/XLSX (pandas-like parsing via .NET libraries), with validation report. 8 SP
CMS‑105: Asset upload API to Blob Storage; signed URLs; antivirus scanning hook. 8 SP
CMS‑106: License metadata; source tracking; embargo flags. 5 SP
CMS‑107: Editorial UI (filters, search, batch tagging). 13 SP
CMS‑108: Exposure control: approved content only visible to assessments. 5 SP

Dependencies: Storage (PLT‑104), Auth roles (AUTH‑104).

Epic D: Assessment & Adaptive Engine (ASS)
Outcome: Generate tests, auto-grade, update learner model.
D‑US‑001: As a learner, I can generate a personalized test for my level
Acceptance Criteria

Generates test honoring constraints: #items, skills balance, difficulty band.
Time limit enforced (optional), randomization with exposure control.

Technical Tasks

ASS‑101: Test templates model (TestTemplate with constraints). 5 SP
ASS‑102: Item selection algorithm (rule-based): difficulty in [level−1, level+1], topic/skill balance, avoid recent repeats. 13 SP
ASS‑103: Test session lifecycle: start, get items, submit, score, complete. 8 SP
ASS‑104: Auto‑grading for MCQ/cloze/matching/short answers (keyword/regex). 13 SP
ASS‑105: Feedback generation (explanations, transcripts, remediation links). 5 SP
ASS‑106: Learner level update logic (score & timing thresholds). 8 SP
ASS‑107: Spaced repetition queue for missed items. 8 SP
ASS‑108: Performance constraints: API P95 < 300ms (cached reads), paging. 5 SP
ASS‑109: Educator assignment API (Phase 2). 8 SP

Dependencies: CMS content (CMS‑102/108), Profiles (PROF‑101).

Epic E: Machine TTS Service (TTS)
Outcome: On-demand audio generation with caching and quotas.
E‑US‑001: As a learner, I can generate machine audio for selected text
Acceptance Criteria

POST /tts/generate returns audio URI; cache hit returns instantly.
Enforces quota per plan; logs usage minutes.

Technical Tasks

TTS‑101: TTS request model + cache table (text_hash, voice, language). 3 SP
TTS‑102: Azure Speech client integration; voice selection; speed/pitch options. 8 SP
TTS‑103: Text normalization (SSML escaping, punctuation, sentence chunking). 5 SP
TTS‑104: Blob storage write; content-type; CDN-cached signed URLs. 5 SP
TTS‑105: Quotas & rate limiting per plan (middleware + DB counters). 8 SP
TTS‑106: Background worker for long texts; queue with Azure Service Bus. 8 SP
TTS‑107: Cost telemetry (characters → cost est.), usage analytics. 5 SP
TTS‑108: Language/voice catalogue API for UI. 3 SP

Dependencies: Storage/CDN, Billing plans (BILL‑101/102).

Epic F: Native Voiceover Marketplace (VO)
Outcome: Paid local/native actor recordings with bid/assign/deliver/revise flow.
F‑US‑001: As a learner, I can order a native voiceover for my text
Acceptance Criteria

Order with text, language, accent, style, speed, format, deadline.
Price quote shows before checkout; after payment, order moves to open queue.

Technical Tasks

VO‑101: Entities: VOOrder, VOBid, VOAssignment, VOAsset, VOActor, VORevision. 8 SP
VO‑102: Pricing engine (base + per word + rush + mastering). 8 SP
VO‑103: Order API & UI (create, view, status timeline). 8 SP
VO‑104: Actor onboarding: KYC status, rate card, supported languages/accents, sample portfolio. 8 SP
VO‑105: Matching: open bidding or auto-assign to curated list; notifications. 8 SP
VO‑106: Secure upload for audio; preview streaming with watermark/beep inserts; final delivery on acceptance. 8 SP
VO‑107: Revision loop: timestamped comments; versioned assets. 8 SP
VO‑108: Payout scheduling (on acceptance) + fee calculation. 8 SP
VO‑109: Dispute handling & refunds; admin tools. 8 SP

Dependencies: Payments (BILL‑103/104), Notifications (NOTIF‑101), Storage.

Epic G: Payments & Billing (BILL)
Outcome: Subscriptions, one-off purchases, VAT, payouts (actors).
G‑US‑001: As a user, I can subscribe to a plan and see my limits
Acceptance Criteria

Plans enforce feature flags & quotas.
Stripe webhook updates subscription state idempotently.

Technical Tasks

BILL‑101: Plans & features model; feature checks middleware. 5 SP
BILL‑102: Checkout sessions for subscriptions; manage free → paid upgrade. 8 SP
BILL‑103: One‑off payments for VO orders (pre-authorize escrow). 8 SP
BILL‑104: Webhooks: invoice.paid, customer.subscription.updated, idempotent handlers. 8 SP
BILL‑105: VAT handling (EU/NO): VAT number validation, invoice tax lines. 8 SP
BILL‑106: Receipts & invoices (PDF generation), email dispatch. 5 SP
BILL‑107: Payouts to actors (Stripe Connect): onboarding, KYC/AML, transfers. 13 SP
BILL‑108: Dunning flows for failed payments. 5 SP

Dependencies: Auth (user identity), VO (order value), TTS (quota checks).

Epic H: Notifications (NOTIF)
Outcome: Email & in‑app notifications for status changes and reminders.
H‑US‑001: As a user, I receive emails when orders update or tests are assigned
Acceptance Criteria

Templated emails, localized; notification preferences.

Technical Tasks

NOTIF‑101: Notification service abstraction; providers (SendGrid/SES). 5 SP
NOTIF‑102: Templates (Razor Class Library or Handlebars), i18n. 5 SP
NOTIF‑103: In‑app notifications feed & read/unread states. 5 SP
NOTIF‑104: Event subscriptions from event bus (Assessment complete, VO status, Billing). 8 SP
NOTIF‑105: Digest emails & quiet hours. 5 SP

Dependencies: Event Bus (PLT‑106), i18n (PROF‑104).

Epic I: Analytics & Reporting (ANL)
Outcome: Learner progress, educator cohort analytics, admin KPIs.
I‑US‑001: As a learner, I can see progress and strengths/weaknesses
Acceptance Criteria

Charts for skill proficiency trends; time on task; recommendations.

Technical Tasks

ANL‑101: Event schema + ingestion (OpenTelemetry-compatible custom events). 5 SP
ANL‑102: Read models for dashboards (materialized views). 8 SP
ANL‑103: Learner dashboard (charts); educator cohort dashboard. 13 SP
ANL‑104: Admin revenue & SLA metrics; voiceover cycle time, rejection rates. 8 SP
ANL‑105: Export CSV/XLSX for educators. 5 SP

Dependencies: Assessments, Billing, VO events.

Epic J: Admin & Backoffice (ADM)
Outcome: Moderation, refunds, disputes, actor management, feature toggles.
Technical Tasks

ADM‑101: Admin portal shell with RBAC; feature flags (e.g., VoMarketplaceEnabled). 8 SP
ADM‑102: User management: search, lock, reset MFA, role change audit. 8 SP
ADM‑103: Content moderation queue and approvals. 5 SP
ADM‑104: Disputes center (VO) with resolution tools/refunds. 8 SP
ADM‑105: Billing view: subscriptions, invoices, webhook health. 5 SP

Dependencies: Auth roles, CMS, VO, Billing.

Epic K: Platform & DevOps (PLT)
Outcome: CI/CD, infra, security, observability, SRE runbooks.
Technical Tasks

PLT‑101: DB setup (PostgreSQL), migrations pipeline (EF Core). 5 SP
PLT‑102: Redis for cache & distributed locks; connection pooling. 5 SP
PLT‑103: API Gateway/BFF project skeleton; solution structure (modular monolith). 5 SP
PLT‑104: Azure Blob Storage + CDN; SAS policies; lifecycle management. 5 SP
PLT‑105: App configuration + Key Vault for secrets. 5 SP
PLT‑106: Azure Service Bus namespaces, topics/subscriptions, retry policies. 5 SP
PLT‑107: CI/CD (GitHub Actions or Azure DevOps): build, test, code coverage gate, infra as code (Bicep/Terraform). 13 SP
PLT‑108: Observability: OpenTelemetry, logs/metrics/traces, dashboards & alerts. 8 SP
PLT‑109: Security baseline: TLS, HSTS, CSP, dependency scanning (Dependabot/Snyk), SAST/DAST. 8 SP
PLT‑110: Backup & restore drills: DB snapshots; Blob immutability policies. 5 SP
PLT‑111: Performance/load testing harness (k6/Locust), targets per NFR. 8 SP


Epic L: Web App (UI/UX) (WEB)
(Assuming React + TypeScript; duplicate similarly for Blazor if chosen)
Technical Tasks

WEB‑101: Design system (Figma → component library), WCAG 2.2 AA baseline. 8 SP
WEB‑102: Routing, auth guards, error boundaries, i18n scaffolding. 5 SP
WEB‑103: Onboarding & placement UI. 8 SP
WEB‑104: Test‑taking UI: timers, item types renderer, keyboard accessibility, audio player. 13 SP
WEB‑105: TTS UI: select text → voice options → generate/play/download (permissions). 8 SP
WEB‑106: VO marketplace UI: order flow, status timeline, messaging thread, revisions. 13 SP
WEB‑107: Subscriptions & checkout UI; receipts. 8 SP
WEB‑108: Dashboards (learner, educator, admin). 13 SP
WEB‑109: Notifications center; preferences. 5 SP
WEB‑110: Localization: nb-NO, en-US; date/number formatting. 5 SP
WEB‑111: A11y QA fixes; screen reader paths for assessments. 8 SP

Dependencies: APIs from other epics.

API Contracts (Initial Stubs)
Plain Texthttp isn’t fully supported. Syntax highlighting is based on Plain Text.POST /auth/loginPOST /auth/registerPOST /auth/refreshGET  /me                # profilePUT  /me                # update preferencesPOST /assessments/placement/startPOST /assessments/tests/generateGET  /assessments/tests/{id}POST /assessments/tests/{id}/submitGET  /assessments/progressGET  /content/items?language=&skill=&difficulty=&q=POST /content/itemsPUT  /content/items/{id}POST /content/importPOST /tts/generate      # body: { text, language, voice, speed?, pitch? }GET  /tts/{id}POST /voiceover/ordersGET  /voiceover/orders/{id}POST /voiceover/orders/{id}/bidPOST /voiceover/orders/{id}/assignPOST /voiceover/orders/{id}/deliverPOST /voiceover/orders/{id}/revisionPOST /voiceover/orders/{id}/acceptPOST /billing/checkout/subscriptionPOST /billing/checkout/oneoffPOST /billing/webhooks/stripeGET  /billing/subscriptions/meGET  /notificationsPOST /notifications/email (admin)Show more lines

Data Model Migrations (Key)
SQL-- UsersCREATE TABLE users (  id UUID PRIMARY KEY, email TEXT UNIQUE NOT NULL, password_hash TEXT,  locale TEXT, role TEXT NOT NULL, preferences JSONB, created_at TIMESTAMPTZ DEFAULT now());-- OrganizationsCREATE TABLE organizations (  id UUID PRIMARY KEY, name TEXT NOT NULL, country TEXT, vat_number TEXT);CREATE TABLE memberships (  user_id UUID REFERENCES users(id), org_id UUID REFERENCES organizations(id),  role TEXT, PRIMARY KEY(user_id, org_id));-- ContentCREATE TABLE passages (  id UUID PRIMARY KEY, language TEXT, text TEXT, tags TEXT[], license JSONB);CREATE TABLE items (  id UUID PRIMARY KEY, language TEXT, skill TEXT, difficulty NUMERIC,  stem JSONB, options JSONB, answer_key JSONB, metadata JSONB,  version INT, status TEXT, updated_at TIMESTAMPTZ DEFAULT now());-- AssessmentsCREATE TABLE test_templates (  id UUID PRIMARY KEY, exam_type TEXT, constraints JSONB, time_limit INT);CREATE TABLE test_sessions (  id UUID PRIMARY KEY, user_id UUID REFERENCES users(id),  template_id UUID REFERENCES test_templates(id),  level_estimate NUMERIC, started_at TIMESTAMPTZ, completed_at TIMESTAMPTZ);CREATE TABLE responses (  id UUID PRIMARY KEY, session_id UUID REFERENCES test_sessions(id),  item_id UUID REFERENCES items(id),  answer JSONB, score NUMERIC, time_spent_ms INT);-- TTSCREATE TABLE tts_cached (  text_hash TEXT PRIMARY KEY, language TEXT, voice TEXT,  audio_uri TEXT, created_at TIMESTAMPTZ DEFAULT now());CREATE TABLE tts_requests (  id UUID PRIMARY KEY, user_id UUID REFERENCES users(id),  text_hash TEXT, voice TEXT, speed NUMERIC, status TEXT,  audio_uri TEXT, engine TEXT, created_at TIMESTAMPTZ DEFAULT now());-- VoiceoverCREATE TABLE vo_orders (  id UUID PRIMARY KEY, user_id UUID REFERENCES users(id), language TEXT, accent TEXT, style TEXT,  speed NUMERIC, format TEXT, word_count INT, price_cents INT, status TEXT, deadline TIMESTAMPTZ, created_at TIMESTAMPTZ DEFAULT now());CREATE TABLE vo_actors (  id UUID PRIMARY KEY, user_id UUID REFERENCES users(id), languages TEXT[], accents TEXT[],  rate_card JSONB, rating NUMERIC, kyc_status TEXT, payout_account JSONB);-- BillingCREATE TABLE plans (  id UUID PRIMARY KEY, name TEXT, features JSONB, price_cents INT, limits JSONB);CREATE TABLE subscriptions (  id UUID PRIMARY KEY, customer_id UUID, plan_id UUID REFERENCES plans(id),  status TEXT, renews_at TIMESTAMPTZ);Show more lines

Definition of Ready (DoR)

User story has business value, clear acceptance criteria, and non-functional constraints.
Designs/wireframes available where applicable.
External dependencies identified (e.g., Stripe keys, Azure Speech keys).
Data model impact assessed; migrations drafted.

Definition of Done (DoD)

Code merged with unit tests (≥80% for domain), integration tests for APIs.
Security checks pass (SAST/DAST), performance budgets met.
Observability: logs/metrics/traces in dashboards; alerts configured as needed.
Documentation updated (README, API docs).
Feature behind a toggle if risky.
Deployed to staging with e2e smoke tests passing.


Testing Strategy (By Layer)

Unit: Selection algorithm, quota logic, pricing engine, grading logic.
Integration: Auth flows, Stripe webhook idempotency, TTS cache, Blob storage.
E2E: Onboarding → placement → generate test → TTS → VO order → payment → delivery.
Performance: k6 scripts for P95/P99 API timings; queue/worker stress (TTS/VO).
Security: OWASP top 10, broken auth, SSRF in media upload, RBAC bypass attempts.
Accessibility: Keyboard-only nav, screen reader landmarks, contrast, captions.


Non-Functional Requirements (NFR Tasks)

NFR‑101: API latency P95 tracking; add prom counters/histograms. 3 SP
NFR‑102: Cache strategy (Redis): item bank cache, TTS cache TTLs; cold-start plan. 5 SP
NFR‑103: Rate limiting middleware (IP + user); plan-based. 5 SP
NFR‑104: GDPR compliance: DSR endpoints (export/delete), privacy notices. 8 SP
NFR‑105: WCAG 2.2 AA audit and fixes checklist. 8 SP
NFR‑106: Availability: blue/green deploy, health checks, graceful shutdown. 5 SP


Security & Compliance Tasks

SEC‑101: Data classification & PII inventory. 3 SP
SEC‑102: Encrypt at rest (Postgres TDE/PGCrypto for sensitive columns) + TLS 1.2+. 5 SP
SEC‑103: Key rotation playbook; Key Vault integration. 5 SP
SEC‑104: Audit logs immutable store (append-only table + WORM in Blob). 5 SP
SEC‑105: Abuse/misuse controls: text moderation on VO orders; profanity filter. 5 SP
SEC‑106: KYC/AML for actors (Stripe Connect flows). 5 SP


Milestone & Sprint Plan (Indicative, 2‑week sprints)
M0 (Weeks 0–2) – Inception & Platform

PLT‑103, PLT‑101, PLT‑105, PLT‑107 (scaffold CI/CD), AUTH‑101, AUTH‑103.

Sprint 1

AUTH‑102, AUTH‑104, PROF‑101, PROF‑104, PLT‑108 (basic), PLT‑109 (baseline).
Deliver: Login, roles, localized shell.

Sprint 2

CMS‑101/102/108, WEB‑102, PLT‑104, PLT‑106.
Deliver: Content API & approved content visibility.

Sprint 3

ASS‑101/102/103, WEB‑104 (basic renderer), ANL‑101.
Deliver: Personalized test generation + session lifecycle.

Sprint 4

ASS‑104/105/106, PROF‑102/103, ANL‑102 (learner read model).
Deliver: Auto‑grading, feedback, placement loop.

Sprint 5

TTS‑101/102/103/104, TTS‑105 (quotas), WEB‑105.
Deliver: Machine TTS end-to-end with caching & quotas.

Sprint 6

BILL‑101/102/104/106, WEB‑107; NOTIF‑101/102.
Deliver: Subscriptions, invoices, emails.

Sprint 7

VO‑101/102/103/104, BILL‑103/107, NOTIF‑104.
Deliver: VO ordering, actor onboarding, escrow.

Sprint 8

VO‑105/106/107/108/109, ADM‑101/104, ANL‑104.
Deliver: Full VO lifecycle, disputes, admin KPIs.

Hardening (Sprint 9)

NFRs, SEC, A11y, performance, backup drills, observability dashboards.


Deliverables per Epic

AUTH: Auth service + tokens + roles + orgs + audit.
CMS: Content API + editor UI + approvals + asset mgmt.
ASS: Test generation engine, auto‑grading, feedback, learner model.
TTS: API + cache + quotas + storage + UI integration.
VO: Marketplace APIs/UI, actor onboarding, revision loop, payouts.
BILL: Plans, subscriptions, invoices, webhooks, dunning.
NOTIF: Email templates, event-driven notifications, in-app feed.
ANL: Dashboards & exports.
ADM: Admin portal with moderation, billing, dispute management.
PLT: Infra as code, CI/CD, SRE runbooks, observability.


Risks & Mitigations (Actionable)

TTS cost spikes → TTS‑105 quotas + TTS‑107 cost telemetry + pre‑gen common passages.
Content quality variance → CMS‑103 approvals + analytics to retire weak items.
Webhook flakiness → BILL‑104 idempotency keys + retry/DLQ + reconciliation job.
Marketplace delays/abuse → VO‑105 SLAs, actor reputation; VO‑109 disputes & escrow.
Latency under load → NFR‑101 metrics + Redis caching + pagination.


Nice-to-haves (Phase 2)

AI‑assisted scoring for essays/speaking (human-in-the-loop).
Mobile PWA packaging.
Elastic/OpenSearch for content discovery.
IRT-based adaptive testing.


What I can generate next for you


.NET 8 solution skeleton with projects and folder structure.
OpenAPI 3.1 spec for the APIs.
Terraform/Bicep for Azure infra (Norway East).



+++++++++++++++++++++++++++++++++++++++++++++


Project Specification: Multilingual Exam Prep Platform (.NET)
1) Vision & Goals
Vision
Build a scalable, multilingual exam preparation platform that delivers personalized tests across proficiency levels and languages, with audio support via machine TTS (free) and native-speaker voiceovers (paid), including a marketplace workflow for local voice actors.
Primary Goals

Improve exam outcomes with adaptive, level‑aligned practice.
Increase engagement through audio content and realistic listening tasks.
Monetize via a dual TTS model: free machine TTS and paid native voiceovers.
Support multiple languages and certification frameworks (e.g., CEFR A1–C2).

Success Metrics (KPIs)

Activation rate (first test within 24h).
Weekly active users (WAU) and retention D7/D30.
Test completion rate and average score progression.
TTS usage minutes (machine vs. native).
Voiceover order conversion, fulfillment SLA, and NPS for recordings.
Revenue: MRR, ARPU, churn.


2) Scope
In Scope (Phase 1–2)

User onboarding with placement test.
Personalized test generation per level (e.g., CEFR) and language.
Question bank with item tagging (skill, difficulty, exam format).
Machine TTS for all texts (usage-limited for free tier).
Native‑speaker voiceover ordering, delivery, and revision workflow.
Payments, invoicing, subscription tiers.
Admin portal for content management and moderation.
Localization of UI and content (multi‑language).
Analytics dashboards for learners and admins.

Out of Scope (Initial)

Live tutoring sessions.
Proctoring and certified exam delivery.
Mobile native apps (will provide responsive web first).


3) Personas & User Journeys
Personas

Learner: Needs targeted practice and audio materials; may order native voiceover for key passages.
Educator/Group Admin: Manages groups, assigns tests, reviews analytics.
Native Voice Actor: Receives voiceover jobs, records, uploads, communicates revisions.
Content Editor: Curates question bank, sets difficulty, validates quality.
Platform Admin: Oversees users, billing, disputes, moderation.

Key Journeys

Onboarding & Placement

Sign up → language(s) & goals → short adaptive placement test → baseline level → first study plan.


Personalized Practice

Select skill (reading/listening/grammar/vocab) → personalized test generated → immediate feedback → spaced repetition recommendations.


Audio Generation

Highlight text → choose machine TTS (free) or order native voiceover (paid) → tracking & delivery.


Voiceover Marketplace

Learner submits text & options (accent, speed, format) → voice actors bid or auto‑assign → delivery & revision → acceptance & payout.




4) Functional Requirements
4.1 Authentication & Authorization

Email/password, OAuth (Microsoft, Google, Apple).
Roles: Learner, Educator, Voice Actor, Content Editor, Admin.
Organization/Group accounts (education/corporate).
MFA (optional), password reset, session management.

4.2 User Profile & Preferences

Target exam(s), language(s), proficiency goals.
Preferred accent and speaking speed.
Localization: UI language, date/number formats (support nb-NO, en-US, etc.).

4.3 Placement & Adaptive Engine

Short adaptive assessment to estimate CEFR level.
Item response theory (IRT) or rule‑based difficulty stepping (Phase 1 rule‑based, Phase 2 IRT).
Ongoing recalibration from test performance.

4.4 Test Generation

Templates per exam (e.g., IELTS/TOEFL/DELE/DELF/Goethe/HSK—support evolves).
Item tagging: language, skill, topic, difficulty, metadata (source, copyright).
Constraints: time limit, # of items, weight per skill.
Randomization with exposure control and content balancing.

4.5 Grading & Feedback

Auto‑graded questions: MCQ, cloze, matching, short answers (keyword/regex), listening comprehension.
Human‑graded tasks: speaking prompts, essay—optionally with rubric for educator grading (Phase 2: AI‑assisted draft scoring + human override).
Feedback: explanations, transcripts, tips, links to targeted practice.
Progress tracking: skill‑level proficiency trend, heatmaps.

4.6 Content Management (CMS)

CRUD for questions/items, passages, audio assets, images.
Versioning, approvals, audits.
Bulk imports (CSV/XLSX), batch tagging.
Copyright & licensing management.

4.7 TTS (Machine)

Generate audio for texts in supported languages.
Voice selection (language, gender, style), speed, pitch.
Caching: avoid re‑generating identical content.
Quotas & rate limiting per plan.

4.8 Native Voiceover (Paid)

Order form: text, language, accent, style (formal/casual), speed, file format, deadline.
Pricing engine: base + length (per character/word) + rush + options (noise cleaning, mastering).
Workflow: open → assigned → in progress → delivered → revision → accepted → closed.
Messaging thread between learner and voice actor.
Quality checklist and acceptance criteria.
Dispute handling & refunds (admin).

4.9 Payments & Billing

Subscriptions (free, premium individual, educator seats, enterprise).
One‑off purchases (voiceovers).
Invoicing, receipts, VAT (EU/NO compliance).
Payouts to voice actors (KYC/AML onboarding; region‑specific).

4.10 Analytics & Reporting

Learner dashboard: progress, strengths/weaknesses, time on task.
Educator dashboard: cohort performance, assignments, completion.
Admin: revenue, MAU/WAU, voiceover SLA, rejection rates, abuse monitoring.

4.11 Notifications

Email and in‑app: assignment due, order status, delivery, revisions.
Optional push notifications (Phase 2).

4.12 Accessibility & Compliance

WCAG 2.2 AA targets.
GDPR (esp. EU/EEA, including Norway); data portability & deletion.
Parental consent workflow if minors (Phase 2).


5) Non‑Functional Requirements

Availability: 99.9% (Phase 2: 99.95% for critical services).
Performance: P95 page load < 2.5s; API P95 < 300ms for cached reads.
Scalability: Horizontal scaling for stateless services and queues for audio jobs.
Security: OWASP ASVS L2; encryption in transit (TLS 1.2+), at rest (AES‑256).
Observability: Centralized logs, metrics, distributed tracing, alerting.
Data Retention: Configurable, default 24 months for activity, 7 years for finance.
Localization: Full i18n (resources, date/number, pluralization).
Testability: 80%+ coverage on core services; e2e smoke tests on deploy.


6) Architecture Overview
Style: Modular monolith (Phase 1) with clear domain boundaries; evolve into microservices when needed.
Proposed Domains/Services

Identity & Access Service
Learner & Profiles Service
Assessment & Adaptive Engine
Content (CMS) Service
TTS Service (Machine)
Voiceover Marketplace Service
Payments & Billing Service
Notification Service
Analytics/Events Service
Admin/Backoffice

High-Level Diagram (textual)
[Web/App UI]
   |
[API Gateway / BFF]
   |
   +-- Identity & Access
   +-- Learner/Profile
   +-- Assessment Engine
   +-- Content/CMS
   +-- TTS (Machine)
   +-- Voiceover Marketplace
   +-- Payments/Billing
   +-- Notifications
   +-- Analytics/Event Bus
   +-- Admin
[Shared: DB cluster, Blob Storage, Cache, Queue]


7) Technology Stack

Frontend:

Option A: Blazor WebAssembly + ASP.NET Core (single-stack)
Option B: React + TypeScript for UI, ASP.NET Core for APIs


Backend: .NET 8 / ASP.NET Core, C# 12
Database: PostgreSQL (primary relational), Redis (caching, session)
Storage: Blob storage for audio, images, exports
Search: PostgreSQL full‑text (Phase 1), Elastic/OpenSearch (Phase 2) for content discovery
Queues/Events: Azure Service Bus or RabbitMQ
TTS (Machine): Azure Cognitive Services (extensible to local providers); caching layer
Payments: Stripe/Adyen (subscriptions + payouts; SEPA/Nordics support)
Auth: OpenID Connect, external IdPs (Microsoft, Google, Apple)
Telemetry: OpenTelemetry + Azure Monitor / Grafana
CI/CD: GitHub Actions/Azure DevOps; Blue‑Green or Canary
Hosting: Azure App Service / AKS; CDN for static/audio delivery


Note: Choose Azure resources for best integration with Cognitive Services and EU/Norway data residency options (consider Norway East region).


8) Data Model (Core Entities)
Users & Org

User(id, email, password_hash, role, locale, preferences, created_at)
Organization(id, name, billing_id, country, vat_number)
Membership(user_id, org_id, role)

Assessment

Item(id, language, skill, difficulty, stem, options[], answer_key, metadata)
Passage(id, language, text, tags, license)
TestTemplate(id, exam_type, skills, constraints, time_limit)
TestSession(id, user_id, template_id, level_estimate, started_at, completed_at)
Response(id, session_id, item_id, answer, score, time_spent)

TTS

TTSRequest(id, user_id, text_hash, voice, speed, status, audio_uri, engine, created_at)
TTSCached(text_hash, language, voice, audio_uri, created_at)

Voiceover

VOOrder(id, user_id, language, accent, style, speed, format, word_count, price, status, deadline, created_at)
VOBid(id, order_id, actor_id, price, estimated_delivery, status)
VOAssignment(id, order_id, actor_id, status, started_at, delivered_at)
VOAsset(id, order_id, uri, duration, sample_rate, bitrate)
VORevision(id, order_id, notes, status)
VOActor(id, user_id, languages[], accents[], rate_card, rating, kyc_status, payout_account)

Billing

Plan(id, name, features, price_month, limits)
Subscription(id, user_id|org_id, plan_id, status, renews_at)
Invoice(id, customer_id, amount, currency, vat, status, issued_at)
Payout(id, actor_id, amount, currency, status, processed_at)

Analytics/Events

Event(id, user_id, type, payload, timestamp)


9) API Surface (selected)
Auth

POST /auth/register, POST /auth/login, POST /auth/refresh, POST /auth/mfa/verify

Assessments

POST /assessments/placement/start
POST /assessments/tests/generate
POST /assessments/tests/{id}/submit
GET  /assessments/progress

Content

GET  /content/items?filters
POST /content/items (editor)
PUT  /content/items/{id} (editor)

TTS

POST /tts/generate (checks cache → generate → persist)
GET  /tts/{id}

Voiceover

POST /voiceover/orders
GET  /voiceover/orders/{id}
POST /voiceover/orders/{id}/bid
POST /voiceover/orders/{id}/assign
POST /voiceover/orders/{id}/deliver
POST /voiceover/orders/{id}/revision
POST /voiceover/orders/{id}/accept

Payments

POST /billing/checkout
POST /billing/webhooks/stripe
GET  /billing/subscriptions/me

Notifications

POST /notifications/email, POST /notifications/inapp


10) Personalization & Adaptive Logic (Phase 1)
Rule‑Based Baseline

Start from placement level.
Select items with difficulty ∈ [level−1, level+1], ensure topic diversity and skill coverage.
After each test, adjust user level:

If score ≥ 80% with P95 time under target → increase level by 0.5.
If score ≤ 50% → decrease level by 0.5 (min floor).


Maintain a spaced‑repetition queue for missed items (decay schedule).

Phase 2

Integrate IRT (2PL/3PL) and exposure control.
Bayesian updating for level estimate with confidence intervals.


11) TTS & Voiceover Workflows
Machine TTS

Client requests /tts/generate with text, voice, language.
Service computes text_hash, checks TTSCached.
If cache hit → return audio URI.
Else enqueue job → generate via provider → store blob → update cache → return URI.
Apply quotas per plan; log usage events.

Native Voiceover

Learner submits VOOrder.
Actors get notified (filtered by language/accent/rate).
Actor bids or auto‑assign to pre‑approved actor.
Actor records, uploads VOAsset, marks delivered.
Learner reviews: approve or request revisions with timestamps/notes.
Upon acceptance, payout scheduled; audio licensed to learner under ToS.


12) Plans & Limits (example)

Free:

X tests/month, basic analytics
Machine TTS up to Y minutes/month
No native voiceover orders


Premium Individual:

Unlimited tests, advanced analytics
Machine TTS Z minutes/month
Discounted native voiceovers


Educator/Teams:

Seat-based, assignments, cohort analytics
Shared TTS pool
Priority support


Enterprise/Edu:

SSO, custom contracts, SLAs
Dedicated storage residency options



(Set X/Y/Z after early usage telemetry.)

13) Security, Privacy, Compliance

Data: Encrypt at rest and in transit; PII minimization; access via least privilege.
GDPR: Lawful basis, consent for analytics cookies, DSRs (access/erasure/portability), DPA with processors.
KYC/AML: For voice actor payouts; keep verification status.
Abuse: Text moderation for orders; watermarking/DRM options for previews.
Audit: Actions by admins/editors logged with immutable trail.


14) Observability & Operations

Metrics: API latency, error rates, queue depth, TTS job duration, order cycle time.
Logs: Structured JSON; correlation IDs; PII redaction.
Tracing: Distributed tracing across API, TTS, marketplace, and queue workers.
Alerts: SLA breaches, payment webhook failures, spike in disputes.
Backups: Daily DB snapshots; geo‑redundant storage; restore tests monthly.


15) QA & Acceptance
Testing Strategy

Unit tests for domain logic (≥80% on engine).
Contract tests for public APIs.
Integration tests with TTS mock.
E2E flows (Cypress/Playwright) for onboarding, test taking, TTS, order lifecycle.
Performance tests: load at 1k RPS read, 200 RPS write.
Security tests: SAST, DAST, dependency scanning.

Acceptance Criteria (Samples)

Generate a test with balanced skills per template constraints.
Machine TTS request returns audio URL within ≤ 5s for ≤ 1k chars (warm cache).
Voiceover order can be placed, assigned, delivered, revised, accepted within defined states.
Payment webhook transitions subscription states reliably and idempotently.


16) Delivery Plan & Milestones (Indicative)
M0 (Week 0–2): Inception, backlog, architecture runway, infra baseline
M1 (Week 3–6): Auth, profiles, CMS MVP, item model
M2 (Week 7–10): Test generation + grading, placement MVP
M3 (Week 11–14): Machine TTS integration + caching, quotas
M4 (Week 15–18): Payments + subscriptions, limits enforcement
M5 (Week 19–22): Voiceover marketplace MVP (order → deliver → revise)
M6 (Week 23–26): Analytics dashboards, educator features, polish, beta
Go‑Live (Week 26+): Production hardening, SLAs, marketing

17) Risk Register & Mitigations

Content Quality Variance: Establish editorial guidelines; peer review; item analytics.
TTS Costs: Aggressive caching; batch processing; pre‑generate for common passages; multi‑provider negotiation.
Marketplace Abuse/Delays: Escrow, reputation system, SLAs, reserve fund, dispute policy.
Payment/Compliance Complexity (EU/NO): Use established PSPs with VAT support; consult on VO payouts.
Scalability for Audio: CDN, chunked uploads, background processing, queue autoscaling.


18) Open Questions (to finalize)

Initial set of target exams & languages (prioritize 2–3 for MVP?).
Preferred UI framework (Blazor vs. React) considering your team’s skills.
Data residency requirements (Norway/EU only?).
Voice actor onboarding model (open marketplace vs. curated roster).
Pricing and limits for each plan (trial periods, discounts).
AI‑assisted grading expectations & guardrails (Phase 2 scope).


19) Next Steps

I can translate this into a .NET 8 solution outline (projects, folders, interfaces), an initial data schema (SQL), and a DevOps pipeline (YAML).
If helpful, I’ll also draft API contracts (OpenAPI) and a sample content tagging guide for editors.


Would you like me to tailor this for your current stack (e.g., Blazor vs. React) and pick the initial two languages/exams to target for MVP?


Create a .NET solution outline

Generate API contracts for key endpoints

Design the database schema
## UI Updates (Implemented)
- Dual-language reader: two columns show aligned segments for the target language and the mother language.
- Audio sync: while audio plays, the current segment in both columns is highlighted and auto-scrolls into view; clicking a segment seeks playback.
- Backend contract: `GET /api/v1/transcript` returns an array of segments `{ start, end, target, native }`.
- Static hosting: UI served from [src/ExamPrepar.Api/wwwroot/index.html](src/ExamPrepar.Api/wwwroot/index.html), styles in [src/ExamPrepar.Api/wwwroot/style.css](src/ExamPrepar.Api/wwwroot/style.css), logic in [src/ExamPrepar.Api/wwwroot/app.js](src/ExamPrepar.Api/wwwroot/app.js). Static files enabled in [src/ExamPrepar.Api/Program.cs](src/ExamPrepar.Api/Program.cs#L23-L27).
- Endpoint mapping: transcript endpoint mapped via [src/ExamPrepar.Api/Endpoints/TranscriptEndpoints.cs](src/ExamPrepar.Api/Endpoints/TranscriptEndpoints.cs) and registered in [Program.cs](src/ExamPrepar.Api/Program.cs#L32-L34).

### Try It
- Build and run:
  - `dotnet build`
  - `dotnet run --project src/ExamPrepar.Api --launch-profile http`
- Open UI: http://localhost:5127/
- API JSON: http://localhost:5127/api/v1/transcript

### Notes
- The UI currently uses a sample transcript, then attempts to fetch from the backend and replaces it if available.
- The audio URL can be changed via the input field; segments are time-based and independent of audio file contents.
- EF Core warning about `Exam.Questions` value comparer is harmless for the demo; will be addressed when persisting transcripts.

### Next
- Persist transcripts in DB with EF Core migrations; add `GET/POST` for editing.
- Optional: per-word timestamps for finer highlighting; import/export transcript JSON.

## Completed Tasks (2026-02-17)
- Core: .NET 8 solution skeleton and project wiring (Api, Application, Domain, Infrastructure, UnitTests).
- PLT‑103 (partial): Solution structure for a modular monolith established (projects + references).
- API: Health check endpoint mapped at `/health`.
- Domain: `Exam` entity, `IExamRepository`, and `ExamService` implemented.
- Infrastructure: EF Core 8 + SQLite configured (`ExamDbContext`), `EfExamRepository` wired; DB created on startup.
- Endpoints: `GET /exams`, `GET /exams/{id}`, `POST /exams` available.
- UI: Dual-language reader in [src/ExamPrepar.Api/wwwroot/index.html](src/ExamPrepar.Api/wwwroot/index.html), [style.css](src/ExamPrepar.Api/wwwroot/style.css), [app.js](src/ExamPrepar.Api/wwwroot/app.js); static files enabled.
- Backend/UI integration: `GET /api/v1/transcript` implemented in [src/ExamPrepar.Api/Endpoints/TranscriptEndpoints.cs](src/ExamPrepar.Api/Endpoints/TranscriptEndpoints.cs) and consumed by the UI.
- Tests: Unit test baseline (sanity) passing; solution builds successfully.


---

Feature: Dynamic Topics Generation
- Add Topics dropdown to "Set Up Your Reading" with a static list for now.
- Future requirement: Generate the topics list dynamically based on the chosen country (target language locale) and CEFR level (A1–C1).
- Data source: Curated content taxonomy tagged by country/locale and level; fallback to generic topics when locale-specific content is missing.
- API: `/api/v1/topics?locale=nb-NO&level=B1` returns grouped topics (categories → items) for the wizard.
- UI: Populate topics select via API; preserve grouping (optgroups) and remember last choice in preferences.



