arch snes.cpu
lorom

// Addresses to helper functions for stat tracking
define inc_stat $dfd800         // Inc stat, stat id in A
define dec_stat $dfd840         // Dec stat, stat id in A
define store_stat $dfd880       // Store stat, value in A, stat id in X
define load_stat $dfd8b0        // Load stat, stat id in A, value returned in A

// RTA Timer (timer 1 is frames, and timer 2 is number of times frames rolled over)
define timer1 $05b8
define timer2 $05ba

// Temp variables (define here to make sure they're not reused, make sure they're 2 bytes apart)
define door_timer_tmp $7fff00
define door_adjust_tmp $7fff02
define add_time_tmp $7fff04

// -------------------------------
// HIJACKS
// -------------------------------

// Samus hit a door block (Gamestate change to $09 just before hitting $0a)
org $82e176
    jml door_entered

// Samus gains control back after door (Gamestate change back to $08 after door transition)
org $82e764
    jml door_exited

// Door starts adjusting
org $82e309
    jml door_adjust_start

// Door stops adjusting
org $82e34c
    jml door_adjust_stop


// -------------------------------
// CODE (using bank A1 free space)
// -------------------------------
org $a1ec00
// Helper function to add a time delta, X = stat to add to, A = value to check against
// This uses 4-bytes for each time delta
add_time:
    sta {add_time_tmp}
    lda {timer1}
    sec
    sbc {add_time_tmp}
    sta {add_time_tmp}
    txa
    jsl {load_stat}
    clc
    adc {add_time_tmp}
    bcc +
    jsl {store_stat}    // If carry set, increase the high bits
    inx
    txa
    jsl {inc_stat}
+
    jsl {store_stat}  
    rts


// Samus hit a door block (Gamestate change to $09 just before hitting $0a)
door_entered:
    lda #$0002  // Number of door transitions
    jsl {inc_stat}  

    lda {timer1}
    sta {door_timer_tmp} // Save RTA time to temp variable

    // Run hijacked code and return
    plp
    inc $0998
    jml $82e1b7

// Samus gains control back after door (Gamestate change back to $08 after door transition)
door_exited:
    // Increment saved value with time spent in door transition
    lda {door_timer_tmp}
    ldx #$0003
    jsr add_time

    // Run hijacked code and return
    lda #$0008
    sta $0998
    jml $82e76a

// Door adjust start
door_adjust_start:
    lda {timer1}
    sta {door_adjust_tmp} // Save RTA time to temp variable

    // Run hijacked code and return
    lda #$e310
    sta $099c
    jml $82e30f

// Door adjust stop
door_adjust_stop:
    lda {door_adjust_tmp}
    inc // Add extra frame to time delta so that perfect doors counts as 0
    ldx #$0005
    jsr add_time

    // Run hijacked code and return
    lda #$e353
    sta $099c
    jml $82e352
    