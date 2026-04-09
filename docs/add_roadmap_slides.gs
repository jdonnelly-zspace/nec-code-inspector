/**
 * Add Content Roadmap slides to the NEC Code Inspector presentation.
 *
 * HOW TO USE:
 * 1. In Google Slides, go to Extensions > Apps Script
 * 2. Delete the default code in Code.gs
 * 3. Paste this entire file
 * 4. Click the Run button (play icon) — select addContentRoadmapSlides
 * 5. Authorize when prompted
 * 6. Slides will be added after the existing 10 slides
 */

function addContentRoadmapSlides() {
  var pres = SlidesApp.getActivePresentation();

  // Colors matching the existing theme
  var NAVY = '#1E2761';
  var ICE_BLUE = '#CADCFC';
  var WHITE = '#FFFFFF';
  var LIGHT_GRAY = '#F5F7FA';
  var DARK_TEXT = '#1E293B';
  var MID_GRAY = '#64748B';
  var TEAL = '#065A82';
  var GREEN = '#10B981';
  var AMBER = '#F59E0B';
  var RED = '#EF4444';

  // ========== SLIDE 11: Content Roadmap Title ==========
  var slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(NAVY);

  // Top accent bar
  slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 5).getFill().setSolidFill(TEAL);
  // Bottom accent bar
  slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 400, 720, 5).getFill().setSolidFill(TEAL);

  var title = slide.insertTextBox('Content Roadmap', 60, 100, 600, 60);
  title.getText().getTextStyle().setFontSize(36).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var sub = slide.insertTextBox('NEC Topics Prioritized by zSpace AR/VR Value', 60, 170, 600, 40);
  sub.getText().getTextStyle().setFontSize(20).setForegroundColor(ICE_BLUE).setFontFamily('Calibri');

  var body = slide.insertTextBox(
    'Each topic scored on three axes:\n\n' +
    '   zSpace AR/VR Advantage — How much does 3D interaction improve learning?\n' +
    '   Exam Importance — How heavily tested on licensing exams?\n' +
    '   Safety Impact — How critical for preventing real-world hazards?\n\n' +
    'P0 = Ship first  |  P1 = Alpha  |  P2 = Beta  |  P3 = Release+',
    60, 230, 600, 140);
  body.getText().getTextStyle().setFontSize(14).setForegroundColor(ICE_BLUE).setFontFamily('Calibri');

  // ========== SLIDE 12: P0 — Ship First Overview ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(LIGHT_GRAY);

  // Header bar
  var header = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 55);
  header.getFill().setSolidFill(NAVY);
  var ht = slide.insertTextBox('P0 — Ship First: The Core Product', 50, 8, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var intro = slide.insertTextBox('These topics deliver the strongest "you can\'t learn this from a textbook" moment on zSpace. P0 covers ~64% of licensing exam weight.', 50, 65, 620, 30);
  intro.getText().getTextStyle().setFontSize(12).setForegroundColor(MID_GRAY).setFontFamily('Calibri');

  // 5 cards in a grid
  var topics = [
    {name: 'Electrical Panel\nInternals & Design', scores: 'zSpace: 5 | Exam: 5 | Safety: 5', color: TEAL,
     desc: 'Open panels with stylus, inspect bus bars, trace circuits in stereo 3D. The #1 demo moment.'},
    {name: 'Working Space\n& Clearances', scores: 'zSpace: 5 | Exam: 4 | Safety: 5', color: TEAL,
     desc: '3D clearance zone volumes rendered around equipment. See violations from every angle.'},
    {name: 'GFCI & AFCI\nProtection', scores: 'zSpace: 4 | Exam: 5 | Safety: 5', color: GREEN,
     desc: 'Room-by-room walkthrough inspecting receptacles in context. Most-tested safety topic.'},
    {name: 'Grounding\n& Bonding', scores: 'zSpace: 5 | Exam: 5 | Safety: 5', color: GREEN,
     desc: 'X-ray view through foundations. Make invisible underground systems visible in 3D.'},
    {name: 'Conductor Sizing &\nOvercurrent Protection', scores: 'zSpace: 3 | Exam: 5 | Safety: 5', color: AMBER,
     desc: 'Trace wires, measure with virtual multimeter, verify at panel. #1 exam topic.'}
  ];

  for (var i = 0; i < topics.length; i++) {
    var col = i % 3;
    var row = Math.floor(i / 3);
    var x = 50 + col * 215;
    var y = 105 + row * 155;
    var w = 200;
    var h = 140;

    // Card background
    var card = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, x, y, w, h);
    card.getFill().setSolidFill(WHITE);
    card.getBorder().getLineFill().setSolidFill(LIGHT_GRAY);

    // Accent top bar
    var accent = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, x, y, w, 4);
    accent.getFill().setSolidFill(topics[i].color);
    accent.getBorder().setTransparent();

    // Title
    var cardTitle = slide.insertTextBox(topics[i].name, x + 8, y + 10, w - 16, 35);
    cardTitle.getText().getTextStyle().setFontSize(12).setBold(true).setForegroundColor(NAVY).setFontFamily('Calibri');

    // Scores
    var cardScores = slide.insertTextBox(topics[i].scores, x + 8, y + 48, w - 16, 15);
    cardScores.getText().getTextStyle().setFontSize(8).setForegroundColor(topics[i].color).setFontFamily('Calibri');

    // Description
    var cardDesc = slide.insertTextBox(topics[i].desc, x + 8, y + 68, w - 16, 65);
    cardDesc.getText().getTextStyle().setFontSize(9).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');
  }

  // ========== SLIDE 13: P0 Detail — Panel & Grounding (zSpace Moments) ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(NAVY);
  slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 5).getFill().setSolidFill(TEAL);

  ht = slide.insertTextBox('P0: The zSpace Advantage', 50, 15, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  // Left card — Panel
  var panelCard = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 50, 65, 300, 190);
  panelCard.getFill().setSolidFill('#273372');
  panelCard.getBorder().setTransparent();

  var pt = slide.insertTextBox('Electrical Panel Inspection', 60, 72, 280, 25);
  pt.getText().getTextStyle().setFontSize(16).setBold(true).setForegroundColor(TEAL).setFontFamily('Calibri');

  var pb = slide.insertTextBox(
    'Student opens panel door with stylus, leans in to read breaker labels in stereo 3D, traces a wire from breaker to its junction box, uses virtual multimeter to test voltage.\n\n' +
    'No other training tool offers this level of hands-on electrical panel access without live voltage risk.',
    60, 100, 280, 145);
  pb.getText().getTextStyle().setFontSize(11).setForegroundColor(WHITE).setFontFamily('Calibri');

  // Right card — Grounding
  var gndCard = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 370, 65, 300, 190);
  gndCard.getFill().setSolidFill('#273372');
  gndCard.getBorder().setTransparent();

  var gt = slide.insertTextBox('Grounding System X-Ray View', 380, 72, 280, 25);
  gt.getText().getTextStyle().setFontSize(16).setBold(true).setForegroundColor(GREEN).setFontFamily('Calibri');

  var gb = slide.insertTextBox(
    'Transparent earth reveals the 8-foot ground rod depth. Student follows the GEC conductor from panel through the wall to the electrode.\n\n' +
    'A second view shows the water pipe transitioning to plastic — student determines if it qualifies as an electrode (10 ft rule).',
    380, 100, 280, 145);
  gb.getText().getTextStyle().setFontSize(11).setForegroundColor(WHITE).setFontFamily('Calibri');

  // Bottom — Pool zones
  var poolBar = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 50, 270, 620, 80);
  poolBar.getFill().setSolidFill(TEAL);
  poolBar.getBorder().setTransparent();

  var poolTitle = slide.insertTextBox('Swimming Pool Safety Zones (P1)', 60, 275, 600, 20);
  poolTitle.getText().getTextStyle().setFontSize(14).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var poolDesc = slide.insertTextBox(
    'Semi-transparent 3D volumes radiate from pool edge — 5 ft equipment zone, 10 ft receptacle zone, 20 ft overhead clearance cone. ' +
    'Student spots a receptacle inside the restricted zone. Impossible to teach spatially without AR/VR.',
    60, 298, 600, 45);
  poolDesc.getText().getTextStyle().setFontSize(11).setForegroundColor(WHITE).setFontFamily('Calibri');

  // ========== SLIDE 14: P0 Content Built vs Planned ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(LIGHT_GRAY);

  header = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 55);
  header.getFill().setSolidFill(NAVY);
  ht = slide.insertTextBox('P0 Content: Built vs. Planned', 50, 8, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  // Built column
  var builtBg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 50, 65, 300, 280);
  builtBg.getFill().setSolidFill(WHITE);

  var builtAccent = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 50, 65, 300, 4);
  builtAccent.getFill().setSolidFill(GREEN);
  builtAccent.getBorder().setTransparent();

  var builtTitle = slide.insertTextBox('BUILT (in current codebase)', 60, 72, 280, 20);
  builtTitle.getText().getTextStyle().setFontSize(12).setBold(true).setForegroundColor(GREEN).setFontFamily('Calibri');

  var builtContent = slide.insertTextBox(
    '42 NEC articles (Ch 1-4, 6)\n' +
    '22 violation definitions\n' +
    '2 inspection scenarios with generators\n' +
    '1 panel sandbox (200A, 12 circuits, 10 rules)\n' +
    'Load calculator (Art. 220 standard method)\n' +
    'Compliance checker (10 NEC rules)\n' +
    '10 quick reference cards\n' +
    'Certificate system + mastery tracking\n' +
    'Progress dashboard\n' +
    'Audio manager + scene transitions\n' +
    'Main menu + settings + boot sequence\n' +
    '73 total C# scripts',
    60, 95, 280, 240);
  builtContent.getText().getTextStyle().setFontSize(11).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');

  // Planned column
  var plannedBg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 370, 65, 300, 280);
  plannedBg.getFill().setSolidFill(WHITE);

  var plannedAccent = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 370, 65, 300, 4);
  plannedAccent.getFill().setSolidFill(AMBER);
  plannedAccent.getBorder().setTransparent();

  var plannedTitle = slide.insertTextBox('PLANNED (P0 remaining)', 380, 72, 280, 20);
  plannedTitle.getText().getTextStyle().setFontSize(12).setBold(true).setForegroundColor(AMBER).setFontFamily('Calibri');

  var plannedContent = slide.insertTextBox(
    '7 additional NEC articles for P0 topics\n' +
    'Working clearance violations (Art. 110.26)\n' +
    'Dedicated equipment space (Art. 110.26(E))\n' +
    'Illumination of working space (Art. 110.26(D))\n' +
    'Entrance to working space (Art. 110.26(C))\n' +
    'Guarding of live parts (Art. 110.27)\n' +
    'Temperature correction factors (Art. 310.15)\n' +
    'Standard fuse/breaker ratings (Art. 240.6)\n\n' +
    'Unity Editor work:\n' +
    '5 World Labs environments (~$61)\n' +
    '5 Unity scenes with 3D objects\n' +
    'UI prefab wiring\n' +
    'zSpace hardware testing',
    380, 95, 280, 240);
  plannedContent.getText().getTextStyle().setFontSize(11).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');

  // ========== SLIDE 15: P1 — Alpha Milestone ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(LIGHT_GRAY);

  header = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 55);
  header.getFill().setSolidFill(NAVY);
  ht = slide.insertTextBox('P1 — Alpha: Expanding the Experience', 50, 8, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var p1Topics = [
    {name: 'Branch Circuits & Spacing', scores: 'zSpace: 4 | Exam: 4 | Safety: 3', color: TEAL,
     desc: 'Walk through rooms measuring receptacle spacing in 3D. 6-foot wall rule and 24-inch countertop rule learned spatially. 12 violations built.', status: 'BUILT'},
    {name: 'Swimming Pool Zones', scores: 'zSpace: 5 | Exam: 3 | Safety: 5', color: GREEN,
     desc: '3D safety zone volumes around pools — 5 ft, 10 ft, 20 ft radiating zones. Art. 680 coverage. 8 violations planned.', status: 'PLANNED'},
    {name: 'Wiring Methods & NM Cable', scores: 'zSpace: 4 | Exam: 4 | Safety: 3', color: TEAL,
     desc: 'X-ray through walls to see cable runs, stapling, bundling. Count cables through stud holes. Art. 334 coverage. Partial.', status: 'PARTIAL'},
    {name: 'Outdoor & Wet Locations', scores: 'zSpace: 4 | Exam: 3 | Safety: 4', color: AMBER,
     desc: 'Inspect weatherproof covers, in-use covers, wet-rated boxes from multiple angles. Art. 406.9, 410.10. 6 violations planned.', status: 'PLANNED'}
  ];

  for (var j = 0; j < p1Topics.length; j++) {
    var cx = 50 + (j % 2) * 325;
    var cy = 68 + Math.floor(j / 2) * 145;

    var bg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, cx, cy, 310, 130);
    bg.getFill().setSolidFill(WHITE);
    bg.getBorder().getLineFill().setSolidFill(LIGHT_GRAY);

    var acc = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, cx, cy, 4, 130);
    acc.getFill().setSolidFill(p1Topics[j].color);
    acc.getBorder().setTransparent();

    var tn = slide.insertTextBox(p1Topics[j].name, cx + 12, cy + 8, 220, 20);
    tn.getText().getTextStyle().setFontSize(13).setBold(true).setForegroundColor(NAVY).setFontFamily('Calibri');

    // Status badge
    var badgeColor = p1Topics[j].status === 'BUILT' ? GREEN : (p1Topics[j].status === 'PARTIAL' ? AMBER : MID_GRAY);
    var badge = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, cx + 240, cy + 8, 60, 18);
    badge.getFill().setSolidFill(badgeColor);
    badge.getBorder().setTransparent();
    var badgeText = slide.insertTextBox(p1Topics[j].status, cx + 242, cy + 9, 56, 16);
    badgeText.getText().getTextStyle().setFontSize(8).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');
    badgeText.getText().getParagraphStyle().setParagraphAlignment(SlidesApp.ParagraphAlignment.CENTER);

    var sc = slide.insertTextBox(p1Topics[j].scores, cx + 12, cy + 32, 280, 15);
    sc.getText().getTextStyle().setFontSize(9).setForegroundColor(p1Topics[j].color).setFontFamily('Calibri');

    var ds = slide.insertTextBox(p1Topics[j].desc, cx + 12, cy + 50, 280, 72);
    ds.getText().getTextStyle().setFontSize(10).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');
  }

  // ========== SLIDE 16: P2 — Beta Milestone ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(LIGHT_GRAY);

  header = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 55);
  header.getFill().setSolidFill(NAVY);
  ht = slide.insertTextBox('P2 — Beta: Exam Prep Completeness', 50, 8, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var p2Topics = [
    {name: 'Motors & Motor Controls', scores: 'zSpace: 4 | Exam: 5 | Safety: 3',
     desc: 'Trace motor circuit in 3D: compressor > disconnect > controller > overload > panel. 2nd most tested exam topic. Art. 430.'},
    {name: 'Commercial Electrical Room', scores: 'zSpace: 4 | Exam: 4 | Safety: 4',
     desc: '480V switchgear inspection — too dangerous IRL for training. Feeder tracing, 6-disconnect rule, demand calculations. Art. 220, 230.'},
    {name: 'Service Entrance & Metering', scores: 'zSpace: 4 | Exam: 3 | Safety: 4',
     desc: 'Follow path from utility pole to meter to main panel. Service drop clearances, conductor sizing, disconnect. Art. 230.'},
    {name: 'Appliance & Equipment Circuits', scores: 'zSpace: 3 | Exam: 3 | Safety: 3',
     desc: 'Dedicated circuits for water heaters, HVAC, EV charging (Art. 625). Disconnect requirements. Art. 422, 440.'}
  ];

  for (var k = 0; k < p2Topics.length; k++) {
    var px = 50;
    var py = 68 + k * 78;

    var pbg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, px, py, 620, 68);
    pbg.getFill().setSolidFill(WHITE);
    pbg.getBorder().getLineFill().setSolidFill(LIGHT_GRAY);

    var pacc = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, px, py, 4, 68);
    pacc.getFill().setSolidFill(TEAL);
    pacc.getBorder().setTransparent();

    var ptn = slide.insertTextBox(p2Topics[k].name, px + 12, py + 5, 300, 20);
    ptn.getText().getTextStyle().setFontSize(13).setBold(true).setForegroundColor(NAVY).setFontFamily('Calibri');

    var psc = slide.insertTextBox(p2Topics[k].scores, px + 320, py + 5, 290, 20);
    psc.getText().getTextStyle().setFontSize(10).setForegroundColor(TEAL).setFontFamily('Calibri');
    psc.getText().getParagraphStyle().setParagraphAlignment(SlidesApp.ParagraphAlignment.END);

    var pds = slide.insertTextBox(p2Topics[k].desc, px + 12, py + 28, 600, 35);
    pds.getText().getTextStyle().setFontSize(10).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');
  }

  // ========== SLIDE 17: P3 — Release & Beyond ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(LIGHT_GRAY);

  header = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 55);
  header.getFill().setSolidFill(NAVY);
  ht = slide.insertTextBox('P3 — Release & Beyond: Deep Specialization', 50, 8, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  var p3Topics = [
    {name: 'Hazardous Locations', scores: 'zSpace: 5 | Exam: 3 | Safety: 5', color: RED,
     desc: 'Invisible gas/dust classification zones rendered as 3D volumes. Gas station, grain elevator, paint booth. Art. 500-505. Too dangerous to teach hands-on.'},
    {name: 'Solar PV & Energy Storage', scores: 'zSpace: 4 | Exam: 3 | Safety: 4', color: AMBER,
     desc: 'Trace DC circuits from roof panels through combiner box, inverter, to AC panel. Rapid shutdown requirements. Art. 690, 706. Growing fast.'},
    {name: 'Transformers', scores: 'zSpace: 3 | Exam: 4 | Safety: 3', color: MID_GRAY,
     desc: 'Overcurrent protection, vault requirements, connection types. Art. 450. Important for master exam.'},
    {name: 'Healthcare Facilities', scores: 'zSpace: 4 | Exam: 2 | Safety: 5', color: GREEN,
     desc: 'Essential electrical systems, patient care areas, wet procedure locations. Art. 517. Specialized but high safety impact.'}
  ];

  for (var m = 0; m < p3Topics.length; m++) {
    var p3x = 50 + (m % 2) * 325;
    var p3y = 68 + Math.floor(m / 2) * 145;

    var p3bg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, p3x, p3y, 310, 130);
    p3bg.getFill().setSolidFill(WHITE);

    var p3acc = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, p3x, p3y, 310, 4);
    p3acc.getFill().setSolidFill(p3Topics[m].color);
    p3acc.getBorder().setTransparent();

    var p3tn = slide.insertTextBox(p3Topics[m].name, p3x + 10, p3y + 12, 290, 20);
    p3tn.getText().getTextStyle().setFontSize(13).setBold(true).setForegroundColor(NAVY).setFontFamily('Calibri');

    var p3sc = slide.insertTextBox(p3Topics[m].scores, p3x + 10, p3y + 35, 290, 15);
    p3sc.getText().getTextStyle().setFontSize(9).setForegroundColor(p3Topics[m].color).setFontFamily('Calibri');

    var p3ds = slide.insertTextBox(p3Topics[m].desc, p3x + 10, p3y + 55, 290, 68);
    p3ds.getText().getTextStyle().setFontSize(10).setForegroundColor(DARK_TEXT).setFontFamily('Calibri');
  }

  // ========== SLIDE 18: Content Totals & Exam Coverage ==========
  slide = pres.appendSlide(SlidesApp.PredefinedLayout.BLANK);
  slide.getBackground().setSolidFill(NAVY);
  slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 0, 720, 5).getFill().setSolidFill(TEAL);
  slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 0, 400, 720, 5).getFill().setSolidFill(TEAL);

  ht = slide.insertTextBox('Content Summary & Exam Coverage', 50, 15, 620, 40);
  ht.getText().getTextStyle().setFontSize(26).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');

  // Content totals
  var stats = [
    {label: '66', sub: 'NEC Articles\nBuilt', color: TEAL},
    {label: '83', sub: 'NEC Articles\nPlanned', color: AMBER},
    {label: '34', sub: 'Violations\nDefined', color: GREEN},
    {label: '10', sub: 'Scenarios\nPlanned', color: TEAL},
    {label: '64%', sub: 'Exam Weight\nCovered (P0)', color: GREEN}
  ];

  for (var s = 0; s < stats.length; s++) {
    var sx = 50 + s * 130;
    var sBg = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, sx, 65, 118, 80);
    sBg.getFill().setSolidFill('#273372');
    sBg.getBorder().setTransparent();

    var sNum = slide.insertTextBox(stats[s].label, sx + 5, 68, 108, 35);
    sNum.getText().getTextStyle().setFontSize(28).setBold(true).setForegroundColor(stats[s].color).setFontFamily('Calibri');
    sNum.getText().getParagraphStyle().setParagraphAlignment(SlidesApp.ParagraphAlignment.CENTER);

    var sSub = slide.insertTextBox(stats[s].sub, sx + 5, 105, 108, 35);
    sSub.getText().getTextStyle().setFontSize(9).setForegroundColor(ICE_BLUE).setFontFamily('Calibri');
    sSub.getText().getParagraphStyle().setParagraphAlignment(SlidesApp.ParagraphAlignment.CENTER);
  }

  // Exam topic weighting
  var examTitle = slide.insertTextBox('Approximate Licensing Exam Topic Distribution', 50, 160, 620, 20);
  examTitle.getText().getTextStyle().setFontSize(13).setBold(true).setForegroundColor(ICE_BLUE).setFontFamily('Calibri');

  var examTopics = [
    {topic: 'Conductor sizing / ampacity', weight: '~15%', priority: 'P0'},
    {topic: 'Overcurrent protection', weight: '~12%', priority: 'P0'},
    {topic: 'Grounding & bonding', weight: '~12%', priority: 'P0'},
    {topic: 'Branch circuits & receptacles', weight: '~10%', priority: 'P0'},
    {topic: 'Load calculations', weight: '~10%', priority: 'P0'},
    {topic: 'Motors', weight: '~8%', priority: 'P2'},
    {topic: 'Wiring methods', weight: '~8%', priority: 'P1'},
    {topic: 'Services & feeders', weight: '~7%', priority: 'P2'},
    {topic: 'GFCI / AFCI', weight: '~5%', priority: 'P0'},
    {topic: 'Special equipment', weight: '~5%', priority: 'P1'}
  ];

  for (var e = 0; e < examTopics.length; e++) {
    var ecol = e % 2;
    var erow = Math.floor(e / 2);
    var ex = 50 + ecol * 340;
    var ey = 185 + erow * 22;

    var priorityColor = examTopics[e].priority === 'P0' ? GREEN : (examTopics[e].priority === 'P1' ? AMBER : MID_GRAY);

    var eLine = slide.insertTextBox(
      examTopics[e].topic + '  ' + examTopics[e].weight + '  [' + examTopics[e].priority + ']',
      ex, ey, 320, 18);
    eLine.getText().getTextStyle().setFontSize(10).setForegroundColor(WHITE).setFontFamily('Calibri');
  }

  // Bottom callout
  var callout = slide.insertShape(SlidesApp.ShapeType.RECTANGLE, 50, 310, 620, 45);
  callout.getFill().setSolidFill(TEAL);
  callout.getBorder().setTransparent();

  var calloutText = slide.insertTextBox(
    'P0 content covers the top 5 exam categories (~64% of exam weight).\n' +
    'By P2 (Beta), coverage reaches ~92% with motors, wiring methods, and services added.',
    60, 315, 600, 35);
  calloutText.getText().getTextStyle().setFontSize(12).setBold(true).setForegroundColor(WHITE).setFontFamily('Calibri');
  calloutText.getText().getParagraphStyle().setParagraphAlignment(SlidesApp.ParagraphAlignment.CENTER);

  Logger.log('Content Roadmap slides added successfully! (' + (pres.getSlides().length - 10) + ' new slides)');
}
