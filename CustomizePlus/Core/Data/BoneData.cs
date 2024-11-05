using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Utility;

namespace CustomizePlus.Core.Data;

public static class BoneData //todo: DI, do not show IVCS unless IVCS is installed/user enabled it, do not show weapon bones
{
    public enum BoneFamily
    {
        根骨骼,
        摇晃,
        脊柱,
        头发,
        脸部,
        眼部,
        耳朵,
        脸颊,
        嘴唇,
        舌头,
        下颌,
        上身,
        手臂,
        手掌,
        尾巴,
        下身,
        腿部,
        足部,
        耳饰,
        帽子,
        披风,
        盔甲,
        裙子,
        装备,
        归档,
        未知
    }

    //TODO move the csv data to an external (compressed?) file
    private static readonly string[] BoneRawTable =
    {
        //Codename, Display Name, Bone Family, Parent (if any), Mirrored Bone (if any)
		"n_root,根骨骼,Root,TRUE,FALSE,,", "n_hara,腹部,Root,TRUE,FALSE,,", "j_kao,头部,Spine,TRUE,FALSE,j_kubi,",
		"j_kubi,颈部,Spine,TRUE,FALSE,j_sebo_c,", "j_sebo_c,颈椎,Spine,TRUE,FALSE,j_sebo_b,",
		"j_sebo_b,胸椎,Spine,TRUE,FALSE,j_sebo_a,", "j_sebo_a,腰椎,Spine,TRUE,FALSE,j_kosi,",
		"j_kosi,腰部,Spine,TRUE,FALSE,,", "j_kami_a,头发 A,Hair,TRUE,FALSE,j_kao,",
		"j_kami_b,头发 B,Hair,TRUE,FALSE,j_kami_a,", "j_kami_f_l,左前发,Hair,TRUE,FALSE,j_kao,j_kami_f_r",
		"j_kami_f_r,右前发,Hair,TRUE,FALSE,j_kao,j_kami_f_l",
		"j_f_mayu_l,左眉脚,Face,TRUE,FALSE,j_kao,j_f_mayu_r",
		"j_f_mayu_r,右眉脚,Face,TRUE,FALSE,j_kao,j_f_mayu_l",
		"j_f_miken_l,左眉心,Face,TRUE,FALSE,j_kao,j_f_miken_r",
		"j_f_miken_r,右眉心,Face,TRUE,FALSE,j_kao,j_f_miken_l",
		"j_f_memoto,额骨桥,Face,TRUE,FALSE,j_kao,", "j_f_umab_l,左上眼睑,Face,TRUE,FALSE,j_kao,j_f_umab_r",
		"j_f_umab_r,右上眼睑,Face,TRUE,FALSE,j_kao,j_f_umab_l",
		"j_f_dmab_l,左下眼睑,Face,TRUE,FALSE,j_kao,j_f_dmab_r",
		"j_f_dmab_r,右下眼睑,Face,TRUE,FALSE,j_kao,j_f_dmab_l",
        "j_f_eye_l,左眼,Archived,TRUE,FALSE,j_kao,j_f_eye_r", "j_f_eye_r,右眼,Archived,TRUE,FALSE,j_kao,j_f_eye_l",
		"j_f_hoho_l,左颊,Face,TRUE,FALSE,j_kao,j_f_hoho_r",
		"j_f_hoho_r,右颊,Face,TRUE,FALSE,j_kao,j_f_hoho_l",
		"j_f_hige_l,硌狮族左胡须,Face,FALSE,FALSE,j_kao,j_f_hige_r",
		"j_f_hige_r,硌狮族右胡须,Face,FALSE,FALSE,j_kao,j_f_hige_l",
		"j_f_hana,鼻子,Face,TRUE,FALSE,j_kao,", "j_f_lip_l,左唇,Face,TRUE,FALSE,j_kao,j_f_lip_r",
		"j_f_lip_r,右唇,Face,TRUE,FALSE,j_kao,j_f_lip_l", "j_f_ulip_a,上唇 A,Face,TRUE,FALSE,j_kao,",
		"j_f_ulip_b,上唇 B,Face,TRUE,FALSE,j_kao,", "j_f_dlip_a,下唇 A,Face,TRUE,FALSE,j_kao,",
		"j_f_dlip_b,下唇 B,Face,TRUE,FALSE,j_kao,",
		"n_f_lip_l,硌狮族左唇,Face,FALSE,FALSE,j_kao,n_f_lip_r",
		"n_f_lip_r,硌狮族右唇,Face,FALSE,FALSE,j_kao,n_f_lip_l", 
		"n_f_ulip_l,硌狮族左上唇,Face,FALSE,FALSE,j_kao,n_f_ulip_r",
		"n_f_ulip_r,硌狮族右上唇,Face,FALSE,FALSE,j_kao,n_f_ulip_l",
		"j_f_dlip,硌狮族下唇,Face,FALSE,FALSE,j_kao,", "j_ago,下颌,Archived,TRUE,FALSE,j_kao,",
		"j_f_uago,硌狮族上唇A,Face,FALSE,FALSE,j_kao,",
		"j_f_ulip,硌狮族上唇B,Face,FALSE,FALSE,j_kao,",
		"j_mimi_l,左耳,Ears,TRUE,FALSE,j_kao,j_mimi_r", "j_mimi_r,右耳,Ears,TRUE,FALSE,j_kao,j_mimi_l", 
		"j_zera_a_l,兔耳01左 A,Ears,FALSE,FALSE,j_kao,j_zera_a_r", 
		"j_zera_a_r,兔耳01右 A,Ears,FALSE,FALSE,j_kao,j_zera_a_l",
		"j_zera_b_l,兔耳01左 B,Ears,FALSE,FALSE,j_kao,j_zera_b_r",
		"j_zera_b_r,兔耳01右 B,Ears,FALSE,FALSE,j_kao,j_zera_b_l",
		"j_zerb_a_l,兔耳02左 A,Ears,FALSE,FALSE,j_kao,j_zerb_a_r",
		"j_zerb_a_r,兔耳02右 A,Ears,FALSE,FALSE,j_kao,j_zerb_a_l",
		"j_zerb_b_l,兔耳02左 B,Ears,FALSE,FALSE,j_kao,j_zerb_b_r",
		"j_zerb_b_r,兔耳02右 B,Ears,FALSE,FALSE,j_kao,j_zerb_b_l",
		"j_zerc_a_l,兔耳03左 A,Ears,FALSE,FALSE,j_kao,j_zerc_a_r",
		"j_zerc_a_r,兔耳03右 A,Ears,FALSE,FALSE,j_kao,j_zerc_a_l",
		"j_zerc_b_l,兔耳03左 B,Ears,FALSE,FALSE,j_kao,j_zerc_b_r",
		"j_zerc_b_r,兔耳03右 B,Ears,FALSE,FALSE,j_kao,j_zerc_b_l",
		"j_zerd_a_l,兔耳04左 A,Ears,FALSE,FALSE,j_kao,j_zerd_a_r",
		"j_zerd_a_r,兔耳04右 A,Ears,FALSE,FALSE,j_kao,j_zerd_a_l",
		"j_zerd_b_l,兔耳04左 B,Ears,FALSE,FALSE,j_kao,j_zerd_b_r",
		"j_zerd_b_r,兔耳04右 B,Ears,FALSE,FALSE,j_kao,j_zerd_b_l",
		"j_sako_l,左锁骨,Chest,TRUE,FALSE,j_sebo_c,j_sako_r",
		"j_sako_r,右锁骨,Chest,TRUE,FALSE,j_sebo_c,j_sako_l",
		"j_mune_l,左乳房,Chest,TRUE,FALSE,j_sebo_b,j_mune_r",
		"j_mune_r,右乳房,Chest,TRUE,FALSE,j_sebo_b,j_mune_l",
		"iv_c_mune_l,左乳房 B（IVCS）,Chest,FALSE,TRUE,j_mune_l,iv_c_mune_r",
		"iv_c_mune_r,右乳房 B（IVCS）,Chest,FALSE,TRUE,j_mune_r,iv_c_mune_l",
		"n_hkata_l,左肩,Arms,TRUE,FALSE,j_ude_a_l,n_hkata_r",
		"n_hkata_r,右肩,Arms,TRUE,FALSE,j_ude_a_r,n_hkata_l",
		"j_ude_a_l,左臂,Arms,TRUE,FALSE,j_sako_l,j_ude_a_r",
		"j_ude_a_r,右臂,Arms,TRUE,FALSE,j_sako_r,j_ude_a_l",
		"iv_nitoukin_l,左二头肌（IVCS）,Arms,FALSE,TRUE,j_ude_a_l,iv_nitoukin_r",
		"iv_nitoukin_r,右二头肌（IVCS）,Arms,FALSE,TRUE,j_ude_a_r,iv_nitoukin_l",
		"n_hhiji_l,左肘,Arms,TRUE,FALSE,j_ude_b_l,n_hhiji_r",
		"n_hhiji_r,右肘,Arms,TRUE,FALSE,j_ude_b_r,n_hhiji_l",
		"j_ude_b_l,左前臂,Arms,TRUE,FALSE,j_ude_a_l,j_ude_b_r",
		"j_ude_b_r,右前臂,Arms,TRUE,FALSE,j_ude_a_r,j_ude_b_l",
		"n_hte_l,左手腕,Arms,TRUE,FALSE,j_ude_b_l,n_hte_r",
		"n_hte_r,右手腕,Arms,TRUE,FALSE,j_ude_b_r,n_hte_l", "j_te_l,左手,Hands,TRUE,FALSE,n_hte_l,j_te_r",
		"j_te_r,右手,Hands,TRUE,FALSE,n_hte_r,j_te_l",
		"j_oya_a_l,左拇指 A,Hands,TRUE,FALSE,j_te_l,j_oya_a_r",
		"j_oya_a_r,右拇指 A,Hands,TRUE,FALSE,j_te_r,j_oya_a_l",
		"j_oya_b_l,左拇指 B,Hands,TRUE,FALSE,j_oya_a_l,j_oya_b_r",
		"j_oya_b_r,右拇指 B,Hands,TRUE,FALSE,j_oya_a_r,j_oya_b_l",
		"j_hito_a_l,左食指 A,Hands,TRUE,FALSE,j_te_l,j_hito_a_r",
		"j_hito_a_r,右食指 A,Hands,TRUE,FALSE,j_te_r,j_hito_a_l",
		"j_hito_b_l,左食指 B,Hands,TRUE,FALSE,j_hito_a_l,j_hito_b_r",
		"j_hito_b_r,右食指 B,Hands,TRUE,FALSE,j_hito_a_r,j_hito_b_l",
		"j_naka_a_l,左中指 A,Hands,TRUE,FALSE,j_te_l,j_naka_a_r",
		"j_naka_a_r,右中指 A,Hands,TRUE,FALSE,j_te_r,j_naka_a_l",
		"j_naka_b_l,左中指 B,Hands,TRUE,FALSE,j_naka_a_l,j_naka_b_r",
		"j_naka_b_r,右中指 B,Hands,TRUE,FALSE,j_naka_a_r,j_naka_b_l",
		"j_kusu_a_l,左无名指 A,Hands,TRUE,FALSE,j_te_l,j_kusu_a_r",
		"j_kusu_a_r,右无名指 A,Hands,TRUE,FALSE,j_te_r,j_kusu_a_l",
		"j_kusu_b_l,左无名指 B,Hands,TRUE,FALSE,j_kusu_a_l,j_kusu_b_r",
		"j_kusu_b_r,右无名指 B,Hands,TRUE,FALSE,j_kusu_a_r,j_kusu_b_l",
		"j_ko_a_l,左小指 A,Hands,TRUE,FALSE,j_te_l,j_ko_a_r",
		"j_ko_a_r,右小指 A,Hands,TRUE,FALSE,j_te_r,j_ko_a_l",
		"j_ko_b_l,左小指 B,Hands,TRUE,FALSE,j_ko_a_l,j_ko_b_r",
		"j_ko_b_r,右小指 B,Hands,TRUE,FALSE,j_ko_a_r,j_ko_b_l",
		"iv_hito_c_l,左食指 C（IVCS）,Hands,FALSE,TRUE,j_hito_b_l,iv_hito_c_r",
		"iv_hito_c_r,右食指 C（IVCS）,Hands,FALSE,TRUE,j_hito_b_r,iv_hito_c_l",
		"iv_naka_c_l,左中指 C（IVCS）,Hands,FALSE,TRUE,j_naka_b_l,iv_naka_c_r",
		"iv_naka_c_r,右中指 C（IVCS）,Hands,FALSE,TRUE,j_naka_b_r,iv_naka_c_l",
		"iv_kusu_c_l,左无名指 C（IVCS）,Hands,FALSE,TRUE,j_kusu_b_l,iv_kusu_c_r",
		"iv_kusu_c_r,右无名指 C（IVCS）,Hands,FALSE,TRUE,j_kusu_b_r,iv_kusu_c_l",
		"iv_ko_c_l,左小指 C（IVCS）,Hands,FALSE,TRUE,j_ko_b_l,iv_ko_c_r",
		"iv_ko_c_r,右小指 C（IVCS）,Hands,FALSE,TRUE,j_ko_b_r,iv_ko_c_l", "n_sippo_a,尾巴 A,Tail,FALSE,FALSE,j_kosi,",
		"n_sippo_b,尾巴 B,Tail,FALSE,FALSE,n_sippo_a,", "n_sippo_c,尾巴 C,Tail,FALSE,FALSE,n_sippo_b,",
		"n_sippo_d,尾巴 D,Tail,FALSE,FALSE,n_sippo_c,", "n_sippo_e,尾巴E,Tail,FALSE,FALSE,n_sippo_d,",
		"iv_shiri_l,左臀部（IVCS）,Groin,FALSE,TRUE,j_kosi,iv_shiri_r",
		"iv_shiri_r,右臀部（IVCS）,Groin,FALSE,TRUE,j_kosi,iv_shiri_l", 
		"iv_kougan_l,左阴囊（IVCS）,Groin,FALSE,TRUE,iv_ochinko_a,iv_kougan_r",
		"iv_kougan_r,右阴囊（IVCS）,Groin,FALSE,TRUE,iv_ochinko_a,iv_kougan_l",
		"iv_ochinko_a,阴茎A（IVCS）,Groin,FALSE,TRUE,j_kosi,", "iv_ochinko_b,阴茎B（IVCS）,Groin,FALSE,TRUE,iv_ochinko_a,",
		"iv_ochinko_c,阴茎C（IVCS）,Groin,FALSE,TRUE,iv_ochinko_b,",
		"iv_ochinko_d,阴茎D（IVCS）,Groin,FALSE,TRUE,iv_ochinko_c,",
		"iv_ochinko_e,阴茎E（IVCS）,Groin,FALSE,TRUE,iv_ochinko_d,",
		"iv_ochinko_f,阴茎F（IVCS）,Groin,FALSE,TRUE,iv_ochinko_e,", "iv_omanko,阴道（IVCS）,Groin,FALSE,TRUE,j_kosi,",
		"iv_kuritto,阴蒂（IVCS）,Groin,FALSE,TRUE,iv_omanko,",
		"iv_inshin_l,左阴唇（IVCS）,Groin,FALSE,TRUE,iv_omanko,iv_inshin_r",
		"iv_inshin_r,右阴唇（IVCS）,Groin,FALSE,TRUE,iv_omanko,iv_inshin_l", "iv_koumon,后庭（IVCS）,Groin,FALSE,TRUE,j_kosi,",
		"iv_koumon_l,后庭右 B（IVCS）,Groin,FALSE,TRUE,iv_koumon,iv_koumon_r",
		"iv_koumon_r,后庭左 B（IVCS）,Groin,FALSE,TRUE,iv_koumon,iv_koumon_l", 
		"j_asi_a_l,左腿,Legs,TRUE,FALSE,j_kosi,j_asi_a_r", 
		"j_asi_a_r,右腿,Legs,TRUE,FALSE,j_kosi,j_asi_a_l",
		"j_asi_b_l,左膝盖,Legs,TRUE,FALSE,j_asi_a_l,j_asi_b_r",
		"j_asi_b_r,右膝盖,Legs,TRUE,FALSE,j_asi_a_r,j_asi_b_l",
		"j_asi_c_l,左小腿,Legs,TRUE,FALSE,j_asi_b_l,j_asi_c_r",
		"j_asi_c_r,右小腿,Legs,TRUE,FALSE,j_asi_b_r,j_asi_c_l",
		"j_asi_d_l,左脚,Feet,TRUE,FALSE,j_asi_c_l,j_asi_d_r",
		"j_asi_d_r,右脚,Feet,TRUE,FALSE,j_asi_c_r,j_asi_d_l",
		"j_asi_e_l,左脚趾,Feet,TRUE,FALSE,j_asi_d_l,j_asi_e_r",
		"j_asi_e_r,右脚趾,Feet,TRUE,FALSE,j_asi_d_r,j_asi_e_l",
		"iv_asi_oya_a_l,左大拇指 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_l,iv_asi_oya_a_r",
		"iv_asi_oya_a_r,右大拇指 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_r,iv_asi_oya_a_l",
		"iv_asi_oya_b_l,左大拇指 B（IVCS）,Feet,FALSE,TRUE,j_asi_oya_a_l,iv_asi_oya_b_r",
		"iv_asi_oya_b_r,右大拇指 B（IVCS）,Feet,FALSE,TRUE,j_asi_oya_a_r,iv_asi_oya_b_l",
		"iv_asi_hito_a_l,左二趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_l,iv_asi_hito_a_r",
		"iv_asi_hito_a_r,左二趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_r,iv_asi_hito_a_l",
		"iv_asi_hito_b_l,左二趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_hito_a_l,iv_asi_hito_b_r",
		"iv_asi_hito_b_r,左二趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_hito_a_r,iv_asi_hito_b_l",
		"iv_asi_naka_a_l,左中趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_l,iv_asi_naka_a_r",
		"iv_asi_naka_a_r,右中趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_r,iv_asi_naka_a_l",
		"iv_asi_naka_b_l,左中趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_naka_b_l,iv_asi_naka_b_r",
		"iv_asi_naka_b_r,右中趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_naka_b_r,iv_asi_naka_b_l",
		"iv_asi_kusu_a_l,左次小趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_l,iv_asi_kusu_a_r",
		"iv_asi_kusu_a_r,右次小趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_r,iv_asi_kusu_a_l",
		"iv_asi_kusu_b_l,左次小趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_kusu_a_l,iv_asi_kusu_b_r",
		"iv_asi_kusu_b_r,右次小趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_kusu_a_r,iv_asi_kusu_b_l",
		"iv_asi_ko_a_l,左小趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_l,iv_asi_ko_a_r",
		"iv_asi_ko_a_r,右小趾 A（IVCS）,Feet,FALSE,TRUE,j_asi_e_r,iv_asi_ko_a_l",
		"iv_asi_ko_b_l,左小趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_ko_a_l,iv_asi_ko_b_r",
		"iv_asi_ko_b_r,右小趾 B（IVCS）,Feet,FALSE,TRUE,j_asi_ko_a_r,iv_asi_ko_b_l",
		"j_ex_met_va,面罩,Hat,FALSE,FALSE,j_kao,", "j_ex_met_a,帽子配饰 A,Hat,FALSE,FALSE,j_kao,",
		"j_ex_met_b,帽子配饰 B,Hat,FALSE,FALSE,j_kao,",
		"n_ear_b_l,左耳环 B,Earrings,FALSE,FALSE,n_ear_a_l,n_ear_b_r",
		"n_ear_b_r,右耳环 B,Earrings,FALSE,FALSE,n_ear_a_r,n_ear_b_l",
		"n_ear_a_l,左耳环 A,Earrings,FALSE,FALSE,j_kao,n_ear_a_r",
		"n_ear_a_r,右耳环 A,Earrings,FALSE,FALSE,j_kao,n_ear_a_l", 
        "j_ex_top_a_l,披风左 A,Cape,FALSE,FALSE,j_sebo_c,j_ex_top_a_r",
        "j_ex_top_a_r,披风右 A,Cape,FALSE,FALSE,j_sebo_c,j_ex_top_a_l",
        "j_ex_top_b_l,披风左 B,Cape,FALSE,FALSE,j_ex_top_a_l,j_ex_top_b_r",
        "j_ex_top_b_r,披风右 B,Cape,FALSE,FALSE,j_ex_top_a_r,j_ex_top_b_l",
		"n_kataarmor_l,左肩甲,Armor,FALSE,FALSE,n_hkata_l,n_kataarmor_r",
		"n_kataarmor_r,右肩甲,Armor,FALSE,FALSE,n_hkata_r,n_kataarmor_l",
		"n_hijisoubi_l,左护肘,Armor,FALSE,FALSE,n_hhiji_l,n_hijisoubi_r",
		"n_hijisoubi_r,右护肘,Armor,FALSE,FALSE,n_hhiji_r,n_hijisoubi_l",
		"n_hizasoubi_l,左护膝,Armor,FALSE,FALSE,j_asi_b_l,n_hizasoubi_r",
		"n_hizasoubi_r,右护膝,Armor,FALSE,FALSE,j_asi_b_r,n_hizasoubi_l",
		"j_sk_b_a_l,衣服后侧左 A,Skirt,FALSE,FALSE,j_kosi,j_sk_b_a_r",
		"j_sk_b_a_r,衣服后侧右 A,Skirt,FALSE,FALSE,j_kosi,j_sk_b_a_l",
		"j_sk_b_b_l,衣服后侧左 B,Skirt,FALSE,FALSE,j_sk_b_a_l,j_sk_b_b_r",
		"j_sk_b_b_r,衣服后侧右 B,Skirt,FALSE,FALSE,j_sk_b_a_r,j_sk_b_b_l",
		"j_sk_b_c_l,衣服后侧左 C,Skirt,FALSE,FALSE,j_sk_b_b_l,j_sk_b_c_r",
		"j_sk_b_c_r,衣服后侧右 C,Skirt,FALSE,FALSE,j_sk_b_b_r,j_sk_b_c_l",
		"j_sk_f_a_l,衣服前侧左,Skirt,FALSE,FALSE,j_kosi,j_sk_f_a_r",
		"j_sk_f_a_r,衣服前侧右 A,Skirt,FALSE,FALSE,j_kosi,j_sk_f_a_l",
		"j_sk_f_b_l,衣服前侧左 B,Skirt,FALSE,FALSE,j_sk_f_a_l,j_sk_f_b_r",
		"j_sk_f_b_r,衣服前侧右 B,Skirt,FALSE,FALSE,j_sk_f_a_r,j_sk_f_b_l",
		"j_sk_f_c_l,衣服前侧左 C,Skirt,FALSE,FALSE,j_sk_f_b_l,j_sk_f_c_r",
		"j_sk_f_c_r,衣服前侧右 C,Skirt,FALSE,FALSE,j_sk_f_b_r,j_sk_f_c_l",
		"j_sk_s_a_l,衣服左侧 A,Skirt,FALSE,FALSE,j_kosi,j_sk_s_a_r",
		"j_sk_s_a_r,衣服右侧 A,Skirt,FALSE,FALSE,j_kosi,j_sk_s_a_l",
		"j_sk_s_b_l,衣服左侧 B,Skirt,FALSE,FALSE,j_sk_s_a_l,j_sk_s_b_r",
		"j_sk_s_b_r,衣服右侧 B,Skirt,FALSE,FALSE,j_sk_s_a_r,j_sk_s_b_l",
		"j_sk_s_c_l,衣服左侧 C,Skirt,FALSE,FALSE,j_sk_s_b_l,j_sk_s_c_r",
		"j_sk_s_c_r,衣服右侧 C,Skirt,FALSE,FALSE,j_sk_s_b_r,j_sk_s_c_l", 
		"n_throw,投掷物,Root,FALSE,FALSE,j_kosi,",
		"j_buki_sebo_l,左护胸,Equipment,FALSE,FALSE,j_kosi,j_buki_sebo_r",
		"j_buki_sebo_r,右护胸,Equipment,FALSE,FALSE,j_kosi,j_buki_sebo_l", 
		"j_buki2_kosi_l,左枪套,Equipment,FALSE,FALSE,j_kosi,j_buki2_kosi_r", 
		"j_buki2_kosi_r,右枪套,Equipment,FALSE,FALSE,j_kosi,j_buki2_kosi_l",
		"j_buki_kosi_l,左刀鞘,Equipment,FALSE,FALSE,j_kosi,j_buki_kosi_r",
		"j_buki_kosi_r,右刀鞘,Equipment,FALSE,FALSE,j_kosi,j_buki_kosi_l",
		"n_buki_tate_l,左盾牌,Equipment,FALSE,FALSE,n_hte_l,n_buki_tate_r",
		"n_buki_tate_r,右盾牌,Equipment,FALSE,FALSE,n_hte_r,n_buki_tate_l",
		"n_buki_l,左武器,Equipment,FALSE,FALSE,j_te_l,n_buki_r",
		"n_buki_r,右武器,Equipment,FALSE,FALSE,j_te_r,n_buki_l",

        "j_f_face,面部根「金曦之遗辉」,Face,TRUE,FALSE,j_kao,",
        "j_f_hana,鼻子,Face,TRUE,FALSE,j_kao,",
        "j_f_hana_l,左鼻孔,Face,TRUE,FALSE,j_f_hana,j_f_hana_r",
        "j_f_hana_r,右鼻孔,Face,TRUE,FALSE,j_f_hana,j_f_hana_l",
        "j_f_uhana,鼻梁,Face,TRUE,FALSE,j_f_hana,",
        "j_f_hoho_l,左上颊,Cheeks,TRUE,FALSE,j_f_face,j_f_hoho_r",
        "j_f_hoho_r,右上颊,Cheeks,TRUE,FALSE,j_f_face,j_f_hoho_l",
        "j_f_dhoho_l,左中颊,Cheeks,TRUE,FALSE,j_f_face,j_f_dhoho_r",
        "j_f_dhoho_r,右中颊,Cheeks,TRUE,FALSE,j_f_face,j_f_dhoho_l",
        "j_f_shoho_l,左下颊,Cheeks,TRUE,FALSE,j_f_face,j_f_shoho_r",
        "j_f_shoho_r,右下颊,Cheeks,TRUE,FALSE,j_f_face,j_f_shoho_l",
        "j_f_dmemoto_l,左前颊,Cheeks,TRUE,FALSE,j_f_face,j_f_dmemoto_r",
        "j_f_dmemoto_r,右前颊,Cheeks,TRUE,FALSE,j_f_face,j_f_dmemoto_l",
        "j_f_dmiken_l,左鼻梁,Face,TRUE,FALSE,j_f_face,j_f_dmiken_r",
        "j_f_dmiken_r,右鼻梁,Face,TRUE,FALSE,j_f_face,j_f_dmiken_l",

        "j_f_ago,下颌,Archived,TRUE,FALSE,j_f_face,",
        "j_f_dago,下颌,Jaw,TRUE,FALSE,j_f_face,",
        "j_f_hagukiup,上牙,Jaw,TRUE,FALSE,j_f_face,",
        "j_f_hagukidn,下牙,Jaw,TRUE,FALSE,j_f_face,",
        "j_f_bero_01,舌头 A,Tongue,TRUE,FALSE,j_f_ago,",
        "j_f_bero_02,舌头 B,Tongue,TRUE,FALSE,j_f_bero_01,",
        "j_f_bero_03,舌头 C,Tongue,TRUE,FALSE,j_f_bero_02,",
        "j_f_dmlip_01_l,左外下嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_dmlip_01_r",
        "j_f_dmlip_02_l,左外下嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_dmlip_02_r",
        "j_f_umlip_01_l,左外上嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_umlip_01_r",
        "j_f_umlip_02_l,左外上嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_umlip_02_r",
        "j_f_dmlip_01_r,右外下嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_dmlip_01_l",
        "j_f_dmlip_02_r,右外下嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_dmlip_02_l",
        "j_f_umlip_01_r,右外上嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_umlip_01_l",
        "j_f_umlip_02_r,右外上嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_umlip_02_l",
        "j_f_dlip_01_l,左下嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_dlip_01_r",
        "j_f_dlip_02_l,左下嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_dlip_02_r",
        "j_f_ulip_01_l,左上嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_ulip_01_r",
        "j_f_ulip_02_l,左上嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_ulip_02_r",
        "j_f_dlip_01_r,右下嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_dlip_01_l",
        "j_f_dlip_02_r,右下嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_dlip_02_l",
        "j_f_ulip_01_r,右上嘴唇 A,Lips,TRUE,FALSE,j_f_ago,j_f_ulip_01_l",
        "j_f_ulip_02_r,右上嘴唇 B,Lips,TRUE,FALSE,j_f_ago,j_f_ulip_02_l",
        "j_f_uslip_l,左上嘴角 A,Lips,TRUE,FALSE,j_f_ago,j_f_uslip_r",
        "j_f_dslip_l,左下嘴角 A,Lips,TRUE,FALSE,j_f_ago,j_f_dslip_r",
        "j_f_uslip_r,右上嘴角 A,Lips,TRUE,FALSE,j_f_ago,j_f_uslip_l",
        "j_f_dslip_r,右下嘴角 A,Lips,TRUE,FALSE,j_f_ago,j_f_dslip_l",

        "j_f_mab_l,左眼皮,Archived,TRUE,FALSE,j_f_face,j_f_mab_r",
        "j_f_eyepuru_l,左眼拉动,Eyes,TRUE,FALSE,j_f_face,j_f_eyepuru_r",
        "j_f_mabdn_01_l,左下眼皮,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_01_r",
        "j_f_mabup_01_l,左上眼皮,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_01_r",
        "j_f_mabdn_02out_l,左下外眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_02out_r",
        "j_f_mabdn_03in_l,左下内眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_03in_r",
        "j_f_mabup_02out_l,左上外眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_02out_r",
        "j_f_mabup_03in_l,左上内眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_03in_r",
        "j_f_mab_r,右眼皮,Archived,TRUE,FALSE,j_f_face,j_f_mab_l",
        "j_f_eyepuru_r,右眼拉动,Eyes,TRUE,FALSE,j_f_face,j_f_eyepuru_l",
        "j_f_mabdn_01_r,右下眼皮,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_01_l",
        "j_f_mabup_01_r,右上眼皮,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_01_l",
        "j_f_mabdn_02out_r,右下外眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_02out_l",
        "j_f_mabdn_03in_r,右下内眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabdn_03in_l",
        "j_f_mabup_02out_r,右上外眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_02out_l",
        "j_f_mabup_03in_r,右上内眼角,Eyes,TRUE,FALSE,j_f_face,j_f_mabup_03in_l",
        "j_f_mmayu_l,左中眉,Eyes,TRUE,FALSE,j_f_face,j_f_mmayu_r",
        "j_f_miken_01_l,左内眉1,Eyes,TRUE,FALSE,j_f_mmayu_l,j_f_miken_01_r",
        "j_f_miken_02_l,左内眉2,Eyes,TRUE,FALSE,j_f_miken_01_l,j_f_miken_02_r",
        "j_f_mmayu_r,右中眉,Eyes,TRUE,FALSE,j_f_face,j_f_mmayu_l",
        "j_f_miken_01_r,右内眉1,Eyes,TRUE,FALSE,j_f_mmayu_r,j_f_miken_01_l",
        "j_f_miken_02_r,右内眉2,Eyes,TRUE,FALSE,j_f_miken_01_r,j_f_miken_02_l",

        "butt_l,左臀（Skelomae）,Jiggle,TRUE,FALSE,j_kosi,butt_r",
        "butt_r,右臀（Skelomae）,Jiggle,TRUE,FALSE,j_kosi,butt_l",
        "thigh_l,左腿（Skelomae）,Jiggle,TRUE,FALSE,j_asi_a_l,thigh_r",
        "thigh_r,右腿（Skelomae）,Jiggle,TRUE,FALSE,j_asi_a_r,thigh_l",
        "forebreas_l,左乳尖（Skelomae）,Jiggle,TRUE,FALSE,j_mune_l,forebreas_r",
        "forebreas_r,右乳尖（Skelomae）,Jiggle,TRUE,FALSE,j_mune_r,forebreas_l",
        "belly_sebo_a,上腹（Skelomae）,Jiggle,TRUE,FALSE,j_sebo_a,",
        "belly_kosi,下腹（Skelomae）,Jiggle,TRUE,FALSE,j_kosi,",
        "ya_shiri_phys_l,左臀（YAS）,Jiggle,TRUE,FALSE,j_kosi,ya_shiri_phys_r",
        "ya_shiri_phys_r,右臀（YAS）,Jiggle,TRUE,FALSE,j_kosi,ya_shiri_phys_l",
        "ya_daitai_phys_l,左腿（YAS）,Jiggle,TRUE,FALSE,j_kosi,ya_daitai_phys_r",
        "ya_daitai_phys_r,右腿（YAS）,Jiggle,TRUE,FALSE,j_kosi,ya_daitai_phys_l",
        "ya_fukubu_phys,腹部（YAS）,Jiggle,TRUE,FALSE,j_kosi,",
    };

    public static readonly Dictionary<BoneFamily, string?> DisplayableFamilies = new()
    {
        { BoneFamily.脸颊, null },
        { BoneFamily.下颌, null },
        { BoneFamily.舌头, null },
        { BoneFamily.嘴唇, null },
        { BoneFamily.眼部, null },
        { BoneFamily.脊柱, null },
        { BoneFamily.头发, null },
        { BoneFamily.脸部, null },
        { BoneFamily.耳朵, null },
        { BoneFamily.上身, null },
        { BoneFamily.手臂, null },
        { BoneFamily.手掌, null },
        { BoneFamily.尾巴, null },
        { BoneFamily.下身, "NSFW IVCS 兼容骨骼" },
        { BoneFamily.腿部, null },
        { BoneFamily.足部, null },
        { BoneFamily.耳饰, "一些模组会使用这些骨骼的物理特性" },
        { BoneFamily.帽子, null },
        { BoneFamily.披风, "一些模组会使用这些骨骼的物理特性" },
        { BoneFamily.盔甲, null },
        { BoneFamily.裙子, null },
        { BoneFamily.装备, "这些可能表现地很奇怪" },
        { BoneFamily.摇晃, "供摇晃物理使用的骨骼。\n注意需要使用相关服装或身体模组才有效。" },
        { BoneFamily.归档, "7.0后已失效的骨骼，如果有错误请联系国服维护者。" },
        {
            BoneFamily.未知,
            "这些骨骼无法确认用途。\n如果你能弄清楚它们的用途，请告诉我们，我们会将它们添加到表格中。"
        }
    };

    private static readonly Dictionary<string, BoneDatum> BoneTable = new();

    private static readonly Dictionary<string, string> BoneLookupByDispName = new();

    static BoneData()
    {
        //apparently static constructors are only guaranteed to START before the class is called
        //which can apparently lead to race conditions, as I've found out
        //this lock is to make sure the table is fully initialized before anything else can try to look at it
        lock (BoneTable)
        {
            var rowIndex = 0;
            foreach (var entry in BoneRawTable)
            {
                try
                {
                    var cells = entry.Split(',');
                    var codename = cells[0];
                    var dispName = cells[1];

                    BoneTable[codename] = new BoneDatum(rowIndex, cells);
                    BoneLookupByDispName[dispName] = codename;

                    if (BoneTable[codename].Family == BoneFamily.未知)
                    {
                        throw new Exception("啥玩意儿？");
                    }
                }
                catch
                {
                    throw new InvalidCastException($"无法解析原始骨骼表 @ 行 {rowIndex}");
                }

                ++rowIndex;
            }

            //iterate through the complete collection and assign children to their parents
            foreach (var kvp in BoneTable)
            {
                var datum = BoneTable[kvp.Key];

                datum.Children = BoneTable.Where(x => x.Value.Parent == kvp.Key).Select(x => x.Key).ToArray();

                BoneTable[kvp.Key] = datum;
            }
        }
    }

    public static void LogNewBones(params string[] boneNames)
    {
        var probablyHairstyleBones = boneNames.Where(IsProbablyHairstyle).ToArray();

        foreach (var hairBone in ParseHairstyle(probablyHairstyleBones))
        {
            BoneTable[hairBone.Codename] = hairBone;
        }

        foreach (var boneName in boneNames.Except(BoneTable.Keys))
        {
            var newBone = new BoneDatum
            {
                RowIndex = -1,
                Codename = boneName,
                DisplayName = $"未知 ({boneName})",
                Family = BoneFamily.未知,
                Parent = "j_kosi",
                Children = Array.Empty<string>(),
                MirroredCodename = null
            };
        }
    }

    public static void UpdateParentage(string parentName, string childName)
    {
        var child = BoneTable[childName];
        var parent = BoneTable[parentName];

        child.Parent = parentName;
        parent.Children = parent.Children.Append(childName).Distinct().ToArray();

        BoneTable[childName] = child;
        BoneTable[parentName] = parent;
    }

    public static string GetBoneDisplayName(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) ? row.DisplayName : codename;
    }

    public static string? GetBoneCodename(string boneDisplayName)
    {
        return BoneLookupByDispName.TryGetValue(boneDisplayName, out var name) ? name : null;
    }

    public static List<string> GetBoneCodenames()
    {
        return BoneTable.Keys.ToList();
    }

    public static List<string> GetBoneCodenames(Func<BoneDatum, bool> predicate)
    {
        return BoneTable.Where(x => predicate(x.Value)).Select(x => x.Key).ToList();
    }

    public static List<string> GetBoneDisplayNames()
    {
        return BoneLookupByDispName.Keys.ToList();
    }

    public static BoneFamily GetBoneFamily(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) ? row.Family : BoneFamily.未知;
    }

    public static bool IsDefaultBone(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) && row.IsDefault;
    }

    public static int GetBoneRanking(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) ? row.RowIndex : 0;
    }

    public static bool IsIVCSCompatibleBone(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) && row.IsIVCSCompatible;
    }

    public static string? GetBoneMirror(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) ? row.MirroredCodename : null;
    }

    public static string[] GetChildren(string codename)
    {
        return BoneTable.TryGetValue(codename, out var row) ? row.Children : Array.Empty<string>();
    }

    public static bool IsProbablyHairstyle(string codename)
    {
        return Regex.IsMatch(codename, @"j_ex_h\d\d\d\d_ke_[abcdeflrsu](_[abcdeflrsu])?");
    }

    public static bool IsNewBone(string codename)
    {
        return !BoneTable.ContainsKey(codename);
    }

    private static BoneFamily ParseFamilyName(string n)
    {
        var simplified = n.Split(' ').FirstOrDefault()?.ToLower() ?? string.Empty;

        var fam = simplified switch
        {
            "root"      => BoneFamily.根骨骼,
            "jiggle"    => BoneFamily.摇晃,
            "spine"     => BoneFamily.脊柱,
            "hair"      => BoneFamily.头发,
            "face"      => BoneFamily.脸部,
            "ears"      => BoneFamily.耳朵,
            "chest"     => BoneFamily.上身,
            "arms"      => BoneFamily.手臂,
            "hands"     => BoneFamily.手掌,
            "tail"      => BoneFamily.尾巴,
            "groin"     => BoneFamily.下身,
            "legs"      => BoneFamily.腿部,
            "feet"      => BoneFamily.足部,
            "earrings"  => BoneFamily.耳饰,
            "hat"       => BoneFamily.帽子,
            "cape"      => BoneFamily.披风,
            "armor"     => BoneFamily.盔甲,
            "skirt"     => BoneFamily.裙子,
            "cheeks"    => BoneFamily.脸颊,
            "equipment" => BoneFamily.装备,
            "jaw"       => BoneFamily.下颌,
            "tongue"    => BoneFamily.舌头,
            "lips"      => BoneFamily.嘴唇,
            "eyes"      => BoneFamily.眼部,
            "archived"  => BoneFamily.归档,
            _           => BoneFamily.未知
        };

        return fam;
    }

    public struct BoneDatum : IComparable<BoneDatum>
    {
        public int RowIndex;

        public string Codename;
        public string DisplayName;
        public BoneFamily Family;

        public bool IsDefault;
        public bool IsIVCSCompatible;

        public string? Parent;
        public string? MirroredCodename;

        public string[] Children;

        public BoneDatum(int row, string[] fields)
        {
            RowIndex = row;

            var i = 0;

            Codename = fields[i++];
            DisplayName = fields[i++];

            Family = ParseFamilyName(fields[i++]);

            IsDefault = bool.Parse(fields[i++]);
            IsIVCSCompatible = bool.Parse(fields[i++]);

            Parent = fields[i].IsNullOrEmpty() ? null : fields[i];
            i++;
            MirroredCodename = fields[i].IsNullOrEmpty() ? null : fields[i];
            i++;

            Children = Array.Empty<string>();
        }

        public int CompareTo(BoneDatum other)
        {
            return RowIndex != other.RowIndex
                ? RowIndex.CompareTo(other.RowIndex)
                : string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);
        }
    }

    #region hair stuff

    private static IEnumerable<BoneDatum> ParseHairstyle(params string[] boneNames)
    {
        List<BoneDatum> output = new();

        var index = 0;
        foreach (var style in boneNames.GroupBy(x => Regex.Match(x, @"\d\d\d\d").Value))
        {
            try
            {
                var parsedBones = style.Select(ParseHairBone).ToArray();

                // if any of the first subs is nonstandard letter, we can presume that any bcd... are part of a rising sequence
                var firstAsc =
                    parsedBones.Any(x => x.sub1 is "a" or "c" or "d" or "e");
                //and we can then presume that the second subs are directional
                //or vice versa. the naming conventions aren't really consistent about whether the sequence is first or second

                foreach (var boneInfo in parsedBones)
                {
                    StringBuilder dispName = new();
                    dispName.Append($"发型 #{boneInfo.id}");

                    var sub1 = GetHairBoneSubLabel(boneInfo.sub1, firstAsc);
                    var sub2 = boneInfo.sub2 == null ? null : GetHairBoneSubLabel(boneInfo.sub2, !firstAsc);

                    dispName.Append($" {sub1}");
                    if (sub2 != null)
                    {
                        dispName.Append($" {sub2}");
                    }

                    var result = new BoneDatum
                    {
                        RowIndex = -1,
                        Codename = boneInfo.name,
                        DisplayName = dispName.ToString(),
                        Family = BoneFamily.头发,
                        IsDefault = false,
                        IsIVCSCompatible = false,
                        Parent = "j_kao",
                        Children = Array.Empty<string>(),
                        MirroredCodename = null
                    };

                    output.Add(result);
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.Error($"Failed to dynamically parse bones for hairstyle of '{boneNames[index]}'");
            }

            index++;
        }

        return output;
    }

    private static (string name, int id, string sub1, string? sub2) ParseHairBone(string boneName)
    {
        var groups = Regex.Match(boneName.ToLower(), @"j_ex_h(\d\d\d\d)_ke_([abcdeflrsu])(?:_([abcdeflrsu]))?")
            .Groups;

        var idNo = int.Parse(groups[1].Value);
        var subFirst = groups[2].Value;
        var subSecond = groups[3].Value.IsNullOrWhitespace() ? null : groups[3].Value;

        return (boneName, idNo, subFirst, subSecond);
    }

    private static string GetHairBoneSubLabel(string sub, bool ascending)
    {
        return (sub.ToLower(), ascending) switch
        {
            ("a", _) => "A",
            ("b", true) => "B",
            ("b", false) => "Back",
            ("c", _) => "C",
            ("d", _) => "D",
            ("e", _) => "E",
            ("f", true) => "F",
            ("f", false) => "Front",
            ("l", _) => "Left",
            ("r", _) => "Right",
            ("u", _) => "Upper",
            ("s", _) => "Side",
            (_, true) => "Next",
            (_, false) => "Bone"
        };
    }

    #endregion
}