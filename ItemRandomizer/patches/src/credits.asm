// custom credits
arch snes.cpu
lorom

// Defines for the script and credits data
define set $9a17
define delay $9a0d
define draw $0000
define end $f6fe, $99fe
define blank $1fc0
define row $0040
define pink "table tables/pink.tbl"
define yellow "table tables/yellow.tbl"
define cyan "table tables/cyan.tbl"
define blue "table tables/blue.tbl"
define green "table tables/green.tbl"
define orange "table tables/orange.tbl"
define purple "table tables/purple.tbl"
define big "table tables/big.tbl"

// Hijack the original credits code to read the script from bank $DF
org $8b999b
    jml patch1 

org $8b99e5
    jml patch2

org $8b9a08
    jml patch3

org $8b9a19
    jml patch4

// Hijack after decompression of regular credits tilemaps
org $8be0d1
    jsl copy


// Load credits script data from bank $df instead of $8c
org $dfd500
patch1:
    phb; pea $df00; plb; plb
    lda $0000, y    
    bpl +
    plb
    jml $8b99a0
+
    plb
    jml $8b99aa

patch2:
    sta $0014
    phb; pea $df00; plb; plb
    lda $0002, y    
    plb
    jml $8b99eb

patch3:
    phb; pea $df00; plb; plb
    lda $0000, y
    tay
    plb
    jml $8b9a0c

patch4:
    phb; pea $df00; plb; plb
    lda $0000, y
    plb
    sta $19fb
    jml $8b9a1f

// Copy custom credits tilemap data from $dfe0000,x to $7f2000,x
copy:
    pha
    phx
    ldx #$0000
-
    lda credits, x
    cmp #$0000
    beq +
    sta $7f2000, x
    inx
    inx
    jmp -
+
    lda #$000b
    ldx #$0000
    jsl store_stat

    lda #$0e4d
    ldx #$0001
    jsl store_stat
    
    jsr write_stats
    plx
    pla
    jsl $8b95ce
    rtl

// Draw time as xx:yy:zz
draw_time:
    phx
    phb
    dey; dey; dey; dey; dey; dey // Decrement Y by 3 characters so the time count fits
    pea $7f7f; plb; plb
    sta $004204
    sep #$20
    lda #$3c
    sta $004206
    pha; pla; pha; pla; rep #$20
    lda $004216 // Seconds or Frames
    sta $12
    lda $004214 // First two groups (hours/minutes or minutes/seconds)
    sta $004204
    sep #$20
    lda #$3c
    sta $004206
    pha; pla; pha; pla; rep #$20
    lda $004214 // First group (hours or minutes)
    jsr draw_two
    iny; iny // Skip past separator
    lda $004216 // Second group (minutes or seconds)
    jsr draw_two
    iny; iny
    lda $12 // Last group (seconds or frames)
    jsr draw_two
    plb
    plx
    rts        

// Draw 5-digit value to credits tilemap
// A = number to draw, Y = row address
draw_value:
    phx    
    phb
    pea $7f7f; plb; plb
    sta $004204
    lda #$0000
    sta $16     // Leading zeroes flag
    sep #$20
    lda #$64
    sta $004206
    pha; pla; pha; pla; rep #$20
    lda $004214 // Top three digits
    sta $12
    lda $004216
    sta $14

    lda $12
    jsr draw_three
    lda $14
    jsr draw_two
    plb
    plx
    rts

draw_three:
    sta $004204
    sep #$20
    lda #$64
    sta $004206
    pha; pla; pha; pla; rep #$20
    lda $004214 // Hundreds
    asl
    tax
    cmp $16
    beq +
    lda numbers_top, x
    sta $0034, y
    lda numbers_bot, x
    sta $0074, y
    inc $16
+
    iny; iny // Next number
    lda $004216

draw_two:
    sta $004204
    sep #$20
    lda #$0a
    sta $004206
    pha; pla; pha; pla; rep #$20
    lda $004214
    asl
    tax
    cmp $16
    beq +
    lda numbers_top, x
    sta $0034, y
    lda numbers_bot, x
    sta $0074, y
    inc $16
+
    lda $004216
    asl
    tax
    cmp $16
    beq +
    lda numbers_top, x
    sta $0036, y
    lda numbers_bot, x
    sta $0076, y
    inc $16
+
    iny; iny; iny; iny
    rts

// Loop through stat table and update RAM with numbers representing those stats
write_stats:
    phy
    phb
    php
    pea $dfdf; plb; plb
    rep #$30
    ldx #$0000
    ldy #$0000

.loop:
    // Get pointer to table
    tya
    asl; asl; asl;
    tax

    // Load stat type
    lda stats+4, x
    beq .end
    cmp #$0001
    beq .number
    cmp #$0002
    beq .time
    jmp .continue

.number:
    // Load statistic
    lda stats, x
    jsl load_stat
    pha

    // Load row address
    lda stats+2, x
    tyx
    tay
    pla
    jsr draw_value
    txy
    jmp .continue

.time:
    // Load statistic
    lda stats, x
    jsl load_stat
    pha

    // Load row address
    lda stats+2, x
    tyx
    tay
    pla
    jsr draw_time
    txy
    jmp .continue

.continue:
    iny
    jmp .loop

.end:
    plp
    plb
    ply
    rts

numbers_top:
    dw $0060, $0061, $0062, $0063, $0064, $0065, $0066, $0067, $0068, $0069, $006a, $006b, $006c, $006d, $006e, $006f
numbers_bot:
    dw $0070, $0071, $0072, $0073, $0074, $0075, $0076, $0077, $0078, $0079, $007a, $007b, $007c, $007d, $007e, $007f 

// Save file 1 = 701400
// Save file 2 = 701700
// Save file 3 = 701A00
// Increment Statistic (in A)
org $dfd800
inc_stat:
    phx; phb
    pea $7070; plb; plb
    asl
    tax
    lda $7e0952
    bne +
    inc $1400, x
    jmp .end
+
    cmp #$0001
    bne +
    inc $1700, x
    jmp .end
+
    inc $1a00, x
.end:
    plb
    plx
    rtl

// Decrement Statistic (in A)
org $dfd840
dec_stat:
    phx; phb
    pea $7070; plb; plb
    asl
    tax
    lda $7e0952
    bne +
    dec $1400, x
    jmp .end
+
    cmp #$0001
    bne +
    dec $1700, x
    jmp .end
+
    dec $1a00, x
.end:
    plb
    plx
    rtl

// Store Statistic (value in A, stat in X)
org $dfd880
store_stat:
    pha
    txa
    asl
    tax
    lda $7e0952
    bne +
    pla
    sta $701400, x
    jmp .end
+
    cmp #$0001
    bne +
    pla
    sta $701700, x
    jmp .end
+
    pla
    sta $701a00, x

.end:
    rtl

// Load Statistic (stat in A, returns value in A)
org $dfd8b0
load_stat:
    phx
    asl
    tax
    lda $7e0952
    bne +
    lda $701400, x
    jmp .end
+
    cmp #$0001
    bne +
    lda $701700, x
    jmp .end
+
    lda $701a00, x

.end:
    plx
    rtl

// New credits script in free space of bank $DF
org $dfd91b
script:
    dw {set}, $0002; -
    dw {draw}, {blank}
    dw {delay}, -
    
    // Show a compact version of the original credits so we get time to add more    
    dw {draw}, {row}*0      // SUPER METROID STAFF
    dw {draw}, {blank}
    dw {draw}, {row}*4      // PRODUCER
    dw {draw}, {blank}
    dw {draw}, {row}*7      // MAKOTO KANOH
    dw {draw}, {row}*8      
    dw {draw}, {blank}
    dw {draw}, {row}*9      // DIRECTOR
    dw {draw}, {blank}
    dw {draw}, {row}*10     // YOSHI SAKAMOTO
    dw {draw}, {row}*11     
    dw {draw}, {blank}
    dw {draw}, {row}*12     // BACK GROUND DESIGNERS
    dw {draw}, {blank}
    dw {draw}, {row}*13     // HIROFUMI MATSUOKA
    dw {draw}, {row}*14     
    dw {draw}, {blank}
    dw {draw}, {row}*15     // MASAHIKO MASHIMO
    dw {draw}, {row}*16     
    dw {draw}, {blank}
    dw {draw}, {row}*17     // HIROYUKI KIMURA
    dw {draw}, {row}*18     
    dw {draw}, {blank}
    dw {draw}, {row}*19     // OBJECT DESIGNERS
    dw {draw}, {blank}
    dw {draw}, {row}*20     // TOHRU OHSAWA
    dw {draw}, {row}*21     
    dw {draw}, {blank}
    dw {draw}, {row}*22     // TOMOYOSHI YAMANE
    dw {draw}, {row}*23    
    dw {draw}, {blank}
    dw {draw}, {row}*24     // SAMUS ORIGINAL DESIGNERS
    dw {draw}, {blank}
    dw {draw}, {row}*25     // HIROJI KIYOTAKE
    dw {draw}, {row}*26    
    dw {draw}, {blank}
    dw {draw}, {row}*27     // SAMUS DESIGNER
    dw {draw}, {blank}
    dw {draw}, {row}*28     // TOMOMI YAMANE
    dw {draw}, {row}*29    
    dw {draw}, {blank}
    dw {draw}, {row}*83     // SOUND PROGRAM
    dw {draw}, {row}*107    // AND SOUND EFFECTS
    dw {draw}, {blank}
    dw {draw}, {row}*84     // KENJI YAMAMOTO
    dw {draw}, {row}*85    
    dw {draw}, {blank}
    dw {draw}, {row}*86     // MUSIC COMPOSERS
    dw {draw}, {blank}
    dw {draw}, {row}*84     // KENJI YAMAMOTO
    dw {draw}, {row}*85    
    dw {draw}, {blank}
    dw {draw}, {row}*87     // MINAKO HAMANO
    dw {draw}, {row}*88    
    dw {draw}, {blank}
    dw {draw}, {row}*30     // PROGRAM DIRECTOR
    dw {draw}, {blank}
    dw {draw}, {row}*31     // KENJI IMAI
    dw {draw}, {row}*64    
    dw {draw}, {blank}
    dw {draw}, {row}*65     // SYSTEM COORDINATOR
    dw {draw}, {blank}
    dw {draw}, {row}*66     // KENJI NAKAJIMA
    dw {draw}, {row}*67    
    dw {draw}, {blank}
    dw {draw}, {row}*68     // SYSTEM PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*69     // YOSHIKAZU MORI
    dw {draw}, {row}*70    
    dw {draw}, {blank}
    dw {draw}, {row}*71     // SAMUS PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*72     // ISAMU KUBOTA
    dw {draw}, {row}*73    
    dw {draw}, {blank}
    dw {draw}, {row}*74     // EVENT PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*75     // MUTSURU MATSUMOTO
    dw {draw}, {row}*76    
    dw {draw}, {blank}
    dw {draw}, {row}*77     // ENEMY PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*78     // YASUHIKO FUJI
    dw {draw}, {row}*79    
    dw {draw}, {blank}
    dw {draw}, {row}*80     // MAP PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*81     // MOTOMU CHIKARAISHI
    dw {draw}, {row}*82    
    dw {draw}, {blank}
    dw {draw}, {row}*101    // ASSISTANT PROGRAMMER
    dw {draw}, {blank}
    dw {draw}, {row}*102    // KOUICHI ABE
    dw {draw}, {row}*103   
    dw {draw}, {blank}
    dw {draw}, {row}*104    // COORDINATORS
    dw {draw}, {blank}
    dw {draw}, {row}*105    // KATSUYA YAMANO
    dw {draw}, {row}*106   
    dw {draw}, {blank}
    dw {draw}, {row}*63     // TSUTOMU KANESHIGE
    dw {draw}, {row}*96   
    dw {draw}, {blank}
    dw {draw}, {row}*89    // PRINTED ART WORK
    dw {draw}, {blank}
    dw {draw}, {row}*90    // MASAFUMI SAKASHITA
    dw {draw}, {row}*91   
    dw {draw}, {blank}
    dw {draw}, {row}*92    // YASUO INOUE
    dw {draw}, {row}*93   
    dw {draw}, {blank}
    dw {draw}, {row}*94    // MARY COCOMA
    dw {draw}, {row}*95   
    dw {draw}, {blank}
    dw {draw}, {row}*99    // YUSUKE NAKANO
    dw {draw}, {row}*100   
    dw {draw}, {blank}
    dw {draw}, {row}*108   // SHINYA SANO
    dw {draw}, {row}*109   
    dw {draw}, {blank}
    dw {draw}, {row}*110   // NORIYUKI SATO
    dw {draw}, {row}*111   
    dw {draw}, {blank}
    dw {draw}, {row}*32    // SPECIAL THANKS TO
    dw {draw}, {blank}
    dw {draw}, {row}*33    // DAN OWSEN
    dw {draw}, {row}*34   
    dw {draw}, {blank}
    dw {draw}, {row}*35    // GEORGE SINFIELD
    dw {draw}, {row}*36   
    dw {draw}, {blank}
    dw {draw}, {row}*39    // MASARU OKADA
    dw {draw}, {row}*40   
    dw {draw}, {blank}
    dw {draw}, {row}*43    // TAKAHIRO HARADA
    dw {draw}, {row}*44   
    dw {draw}, {blank}
    dw {draw}, {row}*47    // KOHTA FUKUI
    dw {draw}, {row}*48   
    dw {draw}, {blank}
    dw {draw}, {row}*49    // KEISUKE TERASAKI
    dw {draw}, {row}*50   
    dw {draw}, {blank}
    dw {draw}, {row}*51    // MASARU YAMANAKA
    dw {draw}, {row}*52   
    dw {draw}, {blank}
    dw {draw}, {row}*53    // HITOSHI YAMAGAMI
    dw {draw}, {row}*54   
    dw {draw}, {blank}
    dw {draw}, {row}*57    // NOBUHIRO OZAKI
    dw {draw}, {row}*58   
    dw {draw}, {blank}
    dw {draw}, {row}*59    // KENICHI NAKAMURA
    dw {draw}, {row}*60   
    dw {draw}, {blank}
    dw {draw}, {row}*61    // TAKEHIKO HOSOKAWA
    dw {draw}, {row}*62   
    dw {draw}, {blank}
    dw {draw}, {row}*97    // SATOSHI MATSUMURA
    dw {draw}, {row}*98   
    dw {draw}, {blank}
    dw {draw}, {row}*122   // TAKESHI NAGAREDA
    dw {draw}, {row}*123  
    dw {draw}, {blank}
    dw {draw}, {row}*124   // MASAHIRO KAWANO
    dw {draw}, {row}*125  
    dw {draw}, {blank}
    dw {draw}, {row}*45    // HIRO YAMADA
    dw {draw}, {row}*46  
    dw {draw}, {blank}
    dw {draw}, {row}*112   // AND ALL OF R&D1 STAFFS
    dw {draw}, {row}*113  
    dw {draw}, {blank}
    dw {draw}, {row}*114   // GENERAL MANAGER
    dw {draw}, {blank}
    dw {draw}, {row}*5     // GUMPEI YOKOI
    dw {draw}, {row}*6  
    dw {draw}, {blank}
    dw {draw}, {blank}
    dw {draw}, {blank}
    dw {draw}, {blank}

    // Custom item randomizer credits text        
    dw {draw}, {row}*128
    dw {draw}, {blank}
    dw {draw}, {row}*129
    dw {draw}, {row}*130
    dw {draw}, {blank}
    dw {draw}, {row}*131
    dw {draw}, {row}*132
    dw {draw}, {blank}
    dw {draw}, {row}*133
    dw {draw}, {row}*134
    dw {draw}, {blank}
    dw {draw}, {row}*135
    dw {draw}, {row}*136
    dw {draw}, {blank}
    dw {draw}, {row}*137
    dw {draw}, {blank}
    dw {draw}, {row}*138
    dw {draw}, {blank}
    dw {draw}, {row}*139
    dw {draw}, {row}*140
    dw {draw}, {blank}
    dw {draw}, {row}*141
    dw {draw}, {row}*142
    dw {draw}, {blank}
    dw {draw}, {row}*143
    dw {draw}, {row}*144
    

    // Scroll all text off and end credits
    dw {set}, $0023; -
    dw {draw}, {blank}
    dw {delay}, -    
    dw {end}

org $dfe000
credits:
    // When using big text, it has to be repeated twice, first in UPPERCASE and then in lowercase since it's split into two parts
    // Numbers are mapped in a special way as described below:
    // 0123456789%& '´
    // }!@#$%&/()>~.
    
    {pink}
    dw "     ITEM RANDOMIZER STAFF      " // 128
    {big}
    dw "             TOTAL              " // 129
    dw "             total              " // 130
    dw "            FOOSDA              " // 131
    dw "            foosda              " // 132
    dw "            LEODOX              " // 133
    dw "            leodox              " // 134
    dw "           DESSYREQT            " // 135
    dw "           dessyreqt            " // 136
    {purple}
    dw "      GAMEPLAY STATISTICS       " // 137
    {orange}
    dw "       ENEMIES AND BOSSES       " // 138
    {big}
    dw " MINIBOSSES KILLED              " // 139
    dw " minibosses killed              " // 140
    dw " TIME IN TOURIAN       00'00'00 " // 141
    dw " time in tourian                " // 142
    dw " TIME WASTED CHARGING  00'00^00 " // 143
    dw " time wasted charging           " // 144
    dw $0000 // End of credits tilemap

stats:
    // STATISTIC, TILEMAP ADDRESS, TYPE, UNUSED (0001 = Number, 0002 = Time, 0003 = ?)
    dw 0, {row}*139, 1, 0    // Minibosses killed
    dw 1, {row}*141, 2, 0    // Time spent in Tourian
    dw 2, {row}*143, 2, 0    // Time wasted charging
    dw $0000, $0000, $0000, $0000    // end of table
