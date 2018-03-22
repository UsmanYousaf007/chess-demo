/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-27 17:08:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class CountryFlagSpriteCache : MonoBehaviour
    {
        [System.Serializable]
        private class _CountryFlags
        {
            #pragma warning disable 0649

            // Using underscores since many of the two letter country codes are
            // language reserved keywords.
            [SerializeField] private Sprite _unknown, _ad, _ae, _af, _ag, _ai, _al, _am, _ao, _aq,
            _ar, _as, _at, _au, _aw, _ax, _az, _ba, _bb, _bd, _be, _bf, _bg, _bh,
            _bi, _bj, _bl, _bm, _bn, _bo, _bq, _br, _bs, _bt, _bv, _bw, _by, _bz,
            _ca, _cc, _cd, _cf, _cg, _ch, _ci, _ck, _cl, _cm, _cn, _co, _cr, _cu,
            _cv, _cw, _cx, _cy, _cz, _de, _dj, _dk, _dm, _do, _dz, _ec, _ee, _eg,
            _eh, _er, _es, _et, _fi, _fj, _fk, _fm, _fo, _fr, _ga, _gb, _gd, _ge,
            _gf, _gg, _gh, _gi, _gl, _gm, _gn, _gp, _gq, _gr, _gs, _gt, _gu, _gw,
            _gy, _hk, _hm, _hn, _hr, _ht, _hu, _id, _ie, _il, _im, _in, _io, _iq,
            _ir, _is, _it, _je, _jm, _jo, _jp, _ke, _kg, _kh, _ki, _km, _kn, _kp,
            _kr, _kw, _ky, _kz, _la, _lb, _lc, _li, _lk, _lr, _ls, _lt, _lu, _lv,
            _ly, _ma, _mc, _md, _me, _mf, _mg, _mh, _mk, _ml, _mm, _mn, _mo, _mp,
            _mq, _mr, _ms, _mt, _mu, _mv, _mw, _mx, _my, _mz, _na, _nc, _ne, _nf,
            _ng, _ni, _nl, _no, _np, _nr, _nu, _nz, _om, _pa, _pe, _pf, _pg, _ph,
            _pk, _pl, _pm, _pn, _pr, _ps, _pt, _pw, _py, _qa, _re, _ro, _rs, _ru,
            _rw, _sa, _sb, _sc, _sd, _se, _sg, _sh, _si, _sj, _sk, _sl, _sm, _sn,
            _so, _sr, _ss, _st, _sv, _sx, _sy, _sz, _tc, _td, _tf, _tg, _th, _tj,
            _tk, _tl, _tm, _tn, _to, _tr, _tt, _tv, _tw, _tz, _ua, _ug, _um, _us,
            _uy, _uz, _va, _vc, _ve, _vg, _vi, _vn, _vu, _wf, _ws, _ye, _yt, _za,
            _zm, _zw;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> flags;

            public void Init()
            {
                flags = new Dictionary<string, Sprite>() {
                    { "unknown", _unknown }, { "AD", _ad }, { "AE", _ae },
                    { "AF", _af }, { "AG", _ag }, { "AI", _ai }, { "AL", _al },
                    { "AM", _am }, { "AO", _ao }, { "AQ", _aq }, { "AR", _ar },
                    { "AS", _as }, { "AT", _at }, { "AU", _au }, { "AW", _aw },
                    { "AX", _ax }, { "AZ", _az }, { "BA", _ba }, { "BB", _bb },
                    { "BD", _bd }, { "BE", _be }, { "BF", _bf }, { "BG", _bg },
                    { "BH", _bh }, { "BI", _bi }, { "BJ", _bj }, { "BL", _bl },
                    { "BM", _bm }, { "BN", _bn }, { "BO", _bo }, { "BQ", _bq },
                    { "BR", _br }, { "BS", _bs }, { "BT", _bt }, { "BV", _bv },
                    { "BW", _bw }, { "BY", _by }, { "BZ", _bz }, { "CA", _ca },
                    { "CC", _cc }, { "CD", _cd }, { "CF", _cf }, { "CG", _cg },
                    { "CH", _ch }, { "CI", _ci }, { "CK", _ck }, { "CL", _cl },
                    { "CM", _cm }, { "CN", _cn }, { "CO", _co }, { "CR", _cr },
                    { "CU", _cu }, { "CV", _cv }, { "CW", _cw }, { "CX", _cx },
                    { "CY", _cy }, { "CZ", _cz }, { "DE", _de }, { "DJ", _dj },
                    { "DK", _dk }, { "DM", _dm }, { "DO", _do }, { "DZ", _dz },
                    { "EC", _ec }, { "EE", _ee }, { "EG", _eg }, { "EH", _eh },
                    { "ER", _er }, { "ES", _es }, { "ET", _et }, { "FI", _fi },
                    { "FJ", _fj }, { "FK", _fk }, { "FM", _fm }, { "FO", _fo },
                    { "FR", _fr }, { "GA", _ga }, { "GB", _gb }, { "GD", _gd },
                    { "GE", _ge }, { "GF", _gf }, { "GG", _gg }, { "GH", _gh },
                    { "GI", _gi }, { "GL", _gl }, { "GM", _gm }, { "GN", _gn },
                    { "GP", _gp }, { "GQ", _gq }, { "GR", _gr }, { "GS", _gs },
                    { "GT", _gt }, { "GU", _gu }, { "GW", _gw }, { "GY", _gy },
                    { "HK", _hk }, { "HM", _hm }, { "HN", _hn }, { "HR", _hr },
                    { "HT", _ht }, { "HU", _hu }, { "ID", _id }, { "IE", _ie },
                    { "IL", _il }, { "IM", _im }, { "IN", _in }, { "IO", _io },
                    { "IQ", _iq }, { "IR", _ir }, { "IS", _is }, { "IT", _it },
                    { "JE", _je }, { "JM", _jm }, { "JO", _jo }, { "JP", _jp },
                    { "KE", _ke }, { "KG", _kg }, { "KH", _kh }, { "KI", _ki },
                    { "KM", _km }, { "KN", _kn }, { "KP", _kp }, { "KR", _kr },
                    { "KW", _kw }, { "KY", _ky }, { "KZ", _kz }, { "LA", _la },
                    { "LB", _lb }, { "LC", _lc }, { "LI", _li }, { "LK", _lk },
                    { "LR", _lr }, { "LS", _ls }, { "LT", _lt }, { "LU", _lu },
                    { "LV", _lv }, { "LY", _ly }, { "MA", _ma }, { "MC", _mc },
                    { "MD", _md }, { "ME", _me }, { "MF", _mf }, { "MG", _mg },
                    { "MH", _mh }, { "MK", _mk }, { "ML", _ml }, { "MM", _mm },
                    { "MN", _mn }, { "MO", _mo }, { "MP", _mp }, { "MQ", _mq },
                    { "MR", _mr }, { "MS", _ms }, { "MT", _mt }, { "MU", _mu },
                    { "MV", _mv }, { "MW", _mw }, { "MX", _mx }, { "MY", _my },
                    { "MZ", _mz }, { "NA", _na }, { "NC", _nc }, { "NE", _ne },
                    { "NF", _nf }, { "NG", _ng }, { "NI", _ni }, { "NL", _nl },
                    { "NO", _no }, { "NP", _np }, { "NR", _nr }, { "NU", _nu },
                    { "NZ", _nz }, { "OM", _om }, { "PA", _pa }, { "PE", _pe },
                    { "PF", _pf }, { "PG", _pg }, { "PH", _ph }, { "PK", _pk },
                    { "PL", _pl }, { "PM", _pm }, { "PN", _pn }, { "PR", _pr },
                    { "PS", _ps }, { "PT", _pt }, { "PW", _pw }, { "PY", _py },
                    { "QA", _qa }, { "RE", _re }, { "RO", _ro }, { "RS", _rs },
                    { "RU", _ru }, { "RW", _rw }, { "SA", _sa }, { "SB", _sb },
                    { "SC", _sc }, { "SD", _sd }, { "SE", _se }, { "SG", _sg },
                    { "SH", _sh }, { "SI", _si }, { "SJ", _sj }, { "SK", _sk },
                    { "SL", _sl }, { "SM", _sm }, { "SN", _sn }, { "SO", _so },
                    { "SR", _sr }, { "SS", _ss }, { "ST", _st }, { "SV", _sv },
                    { "SX", _sx }, { "SY", _sy }, { "SZ", _sz }, { "TC", _tc },
                    { "TD", _td }, { "TF", _tf }, { "TG", _tg }, { "TH", _th },
                    { "TJ", _tj }, { "TK", _tk }, { "TL", _tl }, { "TM", _tm },
                    { "TN", _tn }, { "TO", _to }, { "TR", _tr }, { "TT", _tt },
                    { "TV", _tv }, { "TW", _tw }, { "TZ", _tz }, { "UA", _ua },
                    { "UG", _ug }, { "UM", _um }, { "US", _us }, { "UY", _uy },
                    { "UZ", _uz }, { "VA", _va }, { "VC", _vc }, { "VE", _ve },
                    { "VG", _vg }, { "VI", _vi }, { "VN", _vn }, { "VU", _vu },
                    { "WF", _wf }, { "WS", _ws }, { "YE", _ye }, { "YT", _yt },
                    { "ZA", _za }, { "ZM", _zm }, { "ZW", _zw }
                };
            }

            public Sprite Get(string countryId)
            {
                Assertions.Assert(flags.ContainsKey(countryId), "countryFlags sprite cache doesn't contain a sprite for country ID: " + countryId);
                return flags[countryId];
            }
        }

        [SerializeField] private _CountryFlags countryFlagsSquare;

        public Sprite GetSquare(string countryId)
        {
            return countryFlagsSquare.Get(countryId);
        }

        void Awake()
        {
            countryFlagsSquare.Init();
        }
    }
}
