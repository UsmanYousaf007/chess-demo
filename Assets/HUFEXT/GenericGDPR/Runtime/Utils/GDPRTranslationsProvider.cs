using System.Collections.Generic;
using HUFEXT.CountryCode.Runtime.API;
using HUFEXT.GenericGDPR.Runtime.API;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Runtime.Utils
{
    public static class GDPRTranslationsProvider
    {
        public struct GDPRTranslation
        {
            public string lang;
            public SystemLanguage langType;
            public string header;
            public string policy;
            public string footer;
            public string toggle;
            public string button;
        }

        public const string AD_PARTNERS_LINK = "\"https://huuugegames.com/privacy-policy/#huuuge%27s+partner+list\"";
        public const string PRIVACY_POLICY_LINK = "\"https://huuugegames.com/privacy-policy\"";
        public const string TERMS_OF_USE_LINK = "\"https://www.huuugegames.com/terms-of-use\"";
        
        // First element in list is default translation.
        static readonly List<GDPRTranslation> translations = new List<GDPRTranslation>
        {
            new GDPRTranslation
            {
                lang = "en",
                langType = SystemLanguage.English,
                header = "We Respect Your Privacy",
                policy = "I hereby consent to the usage and disclosure of my personal data (including device information and my preferences) to <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>advertising network companies</u></color></link> for the purpose of serving targeted advertisements to me in the game. I understand that I can withdraw this consent at any time within the game Settings, as also described in our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Privacy Policy</u></color></link>.",
                footer = "By tapping Continue below, you agree to be bound by our <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Terms of Use</u></b></color></link> and our <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Privacy Policy</u></b></color></link> and you affirm that you are older than 16.",
                toggle = "I Accept",
                button = "Continue"
            },

            new GDPRTranslation
            {
                lang = "pl",
                langType = SystemLanguage.Polish,
                header = "Cenimy twoją prywatność",
                policy = "Wyrażam zgodę na używanie i ujawnianie moich danych osobowych (w tym informacje o urządzeniu i moich preferencjach) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>sieciom reklamowym</u></color></link> w celu dostarczania mi dopasowanych reklam w grze. Rozumiem, że mogę wycofać swoją zgodę w dowolnej chwili w Ustawieniach gry, jak wskazano też w <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Polityce Prywatności</u></color></link>.",
                footer = "Klikając Dalej poniżej, akceptujesz nasze <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Warunki Korzystania</u></b></color></link> oraz <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Politykę Prywatności</u></b></color></link> oraz potwierdzasz ukończenie 16-ego roku życia.",
                toggle = "Wyrażam zgodę",
                button = "Kontyntynuj"
            },
            
            new GDPRTranslation()
            {
                lang = "it",
                langType = SystemLanguage.Italian,
                header = "La tua privacy ci sta a cuore",
                policy = @"Acconsento all'utilizzo e alla divulgazione dei miei dati personali (comprese le informazioni sul dispositivo e le mie preferenze) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>alle società della rete pubblicitaria</u></color></link> allo scopo di offrirmi pubblicità mirate nel gioco. Comprendo che posso revocare questo consenso in qualsiasi momento nelle Impostazioni del gioco, come descritto anche nella nostra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Informativa sulla privacy</u></color></link>.",
                footer = @"Toccando Continua di seguito, l'utente accetta di essere vincolato dalle nostre <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Condizioni d'Uso</u></b></color></link> e dalla nostra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Informativa sulla Privacy</u></b></color></link> e afferma di avere più di 16 anni.",
                toggle = "Acepto",
                button = "Continua"
            },
            
            new GDPRTranslation()
            {
                lang = "es",
                langType = SystemLanguage.Spanish,
                header = "Nos importa tu privacidad",
                policy = @"Por la presente, doy mi consentimiento para el uso y divulgación de mis datos personales (incluida la información del dispositivo y mis preferencias) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>a las empresas de publicidad</u></color></link> para recibir publicidad específica acorde a mis preferencias en el juego. Estoy informado de que puedo retirar este consentimiento en cualquier momento en el apartado \"Configuración\" del juego, como, del mismo modo, se expresa en nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link>.",
                footer = @"Tocando Continuar debajo, se acepta estar sujeto a nuestros <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Términos de uso</u></b></color></link> y condiciones y nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Política de privacidad</u></b></color></link> y afirma que es mayor de 16 años.",
                toggle = "Acepto",
                button = "Seguir"
            },
            
            new GDPRTranslation()
            {
                lang = "es-mx",
                langType = SystemLanguage.Unknown,
                header = "Nos importa tu privacidad",
                policy = @"Por la presente doy mi consentimiento para el uso y la divulgación de mis datos personales (incluida la información del dispositivo y mis preferencias) a las <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de publicidad</u></color></link>, con el fin de que me muestren publicidad personalizada en el juego. Entiendo que puedo revocar este consentimiento en cualquier momento dentro de las Configuraciones del juego, como también se describe en nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de privacidad</u></color></link>.",
                footer = @"Tocando Continuar debajo, accede a estar sujeto a nuestras <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Condiciones de uso</u></b></color></link> y a nuestra <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Política de privacidad</u></b></color></link> y afirma que es mayor de 16 años.",
                toggle = "Aceptar",
                button = "Seguir"
            },
            
            new GDPRTranslation()
            {
                lang = "ja",
                langType = SystemLanguage.Japanese,
                header = "あなたのプライバシーを大切にしています",
                policy = @"本サービスを利用されたことに伴い取得したユーザーの情報、デバイス情報及びユーザーに合わせた広告その他の情報を、ユーザーの事前の同意なく<link=" + AD_PARTNERS_LINK + "><color=#{0}><u>第三者</u></color></link>へ提供できるものとします。<link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>プライバシーポリシー</u></color></link>に記載されているようにユーザーはいつでも同意を取り消すことができます。",
                footer = @"下の[続行]をタップすると、<link = "+ TERMS_OF_USE_LINK +"> <color =＃{0}> <b> <u>利用規約</ u> </ b> </ color>に拘束されることに同意したことになります。 </ link>および<link = "+ PRIVACY_POLICY_LINK +"> <color =＃{0}> <b> <u>プライバシーポリシー</ u> </ b> </ color> </ link>を確認すると、 あなたが16歳以上であること",
                toggle = "同意する",
                button = "継続する"
            },
            
            new GDPRTranslation()
            {
                lang = "fr",
                langType = SystemLanguage.French,
                header = "Votre vie privée nous importe",
                policy = @"Je consens par la présente à l'utilisation et à la divulgation de mes données personnelles (y compris les informations relatives à l'appareil et mes préférences) aux <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>sociétés de réseaux publicitaires</u></color></link> dans le but de me faire parvenir des publicités ciblées dans le jeu. Je peux retirer ce consentement à tout moment dans les paramètres du jeu, comme indiqué dans la <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>politique de confidentialité</u></color></link>.",
                footer = @"En appuyant sur Continuer ci-dessous, vous acceptez nos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>conditions d'utilisation</u></b></color></link>  et notre <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>politique de confidentialité</u></b></color></link> et vous affirmez avoir plus de 16 ans.",
                toggle = "J'accepte",
                button = "Continuer"
            },
            
            new GDPRTranslation()
            {
                lang = "de",
                langType = SystemLanguage.German,
                header = "Deine Privatsphäre ist uns wichtig",
                policy = @"Ich stimme hiermit der Verwendung und Weitergabe meiner persönlichen Daten (einschließlich Geräteinformationen und meiner Präferenzen) an <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>Werbenetzwerkunternehmen</u></color></link> zu, um mir im Spiel gezielte Werbung zu liefern. Ich habe verstanden, dass ich diese Einwilligung, wie auch in unserer <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Datenschutzrichtlinie</u></color></link> beschrieben, jederzeit innerhalb der Spieleinstellungen widerrufen kann.",
                footer = @"Durch Tippen auf Weiter unten, erklärst du dich mit unseren <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Nutzungsbedingungen</u></b></color></link> und unserer <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Datenschutzrichtlinie</u></b></color></link> einverstanden und bestätigst, dass du älter als 16 Jahre bist.",
                toggle = "Ich nehme an",
                button = "Fortsetzen"
            },
            
            new GDPRTranslation()
            {
                lang = "ko",
                langType = SystemLanguage.Korean,
                header = "당사는 여러분의 개인정보를 중요하게 생각합니다",
                policy= @"본인은 이로써 게임에서 본인에게 표적 광고를 제공할 목적을 위해 <link=" + AD_PARTNERS_LINK + @"><color=#{0}><u>광고 네트워크 회사</u></color></link>에 내 개인 데이터(장치 정보 및 본인의 선호 포함)를 사용하고 공개하는 데 동의합니다. 본인은 우리의 <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>개인정보보호 정책</u></color></link>에서도 기술된 바와 같이 게임 설정 내에서 언제든지 이 동의를 철회할 수 있습니다.",
                footer = "아래 계속을 탭하면 <link ="+ TERMS_OF_USE_LINK + "> <color = # {0}> <b> <u> 이용 약관 </ u> </ b> <에 동의하게됩니다. / color> </ link> 및 Google의 <link = "+ PRIVACY_POLICY_LINK +"> <color = # {0}> <b> <u> 개인 정보 보호 정책 </ u> </ b> </ color> </ link> 16 세 이상임을 확인합니다. ",                toggle = "동의합니다.",
                button = "계속하다"
            },
            
            new GDPRTranslation()
            {
                lang = "pt",
                langType = SystemLanguage.Portuguese,
                header = "Preocupamo-nos com a tua privacidade",
                policy = @"Concordo pelo presente que o uso e divulgação de meus dados pessoais (incluindo informações de dispositivo e minhas preferências) para <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de rede de publicidade</u></color></link> para o propósito de servir anúncios segmentados para mim no jogo. Entendo que eu posso retirar este consentimento a qualquer hora nas Definições de jogo, como também descrito na nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link>.",
                footer = @"Ao tocar em Continuar abaixo, você concorda em estar ligado aos nossos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Termos de Uso</u></b></color></link> e a nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Política de Privacidade</u></b></color></link> e você afirma que você tem mais de 16 anos.",
                toggle = "Eu aceito",
                button = "Continuar"
            },
            
            new GDPRTranslation()
            {
                lang = "pt-br",
                langType = SystemLanguage.Unknown,
                header = "Nós cuidamos da sua privacidade",
                policy = @"Dou permissão para usar e divulgar meus dados pessoais (incluindo informações do dispositivo e preferências) para <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>empresas de marketing de rede</u></color></link> com o propósito de receber anúncios personalizados no jogo. Eu compreendo que posso retirar esta permissão a qualquer momento nas configurações do jogo, como também está descrito na <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Política de Privacidade</u></color></link>.",
                footer = @"Ao tocar em Continuar abaixo, você concorda com o vínculo dos nossos <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Termos de Uso</u></b></color></link> e nossa <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Política de Privacidade</u></b></color></link>, e confirma ter mais de 16 anos.",
                toggle = "Aceito",
                button = "Continuar"
            },
            
            new GDPRTranslation()
            {
                lang = "ru",
                langType = SystemLanguage.Russian,
                header = "Мы ценим вашу конфиденциальность",
                policy = @"Настоящим я даю согласие на использование и разглашение моих персональных данных (включая информацию об устройстве и о моих предпочтениях) <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>сетевым рекламным компаниям</u></color></link> с целью подачи мне адресной рекламы в процессе игры. Я понимаю, что могу отозвать это свое согласие в любое время в Настройках игры, что также описано в  <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>Политике Конфиденциальности</u></color></link>.",
                footer = @"Нажав Продолжить ниже, ты соглашаешься с нашими <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>Условиями использования и</u></b></color></link> нашей <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>Политикой Конфиденциальности</u></b></color></link>, и подтверждаешь, что тебе больше 16 лет.",
                toggle = "Я принимаю",
                button = "Продолжать"
            },
            
            new GDPRTranslation()
            {
                lang = "nb",
                langType = SystemLanguage.Norwegian,
                header = "Personvernet ditt er viktig for oss",
                policy = @"Med dette gir du samtykke til at dine personopplysninger (deriblant informasjon om enheten din og dine preferanser) kan brukes av og deles med <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>selskaper som driver med annonsenettverk</u></color></link> med formål om å levere målrettet reklame til deg i spillet. Du forstår at du når som helst kan trekke tilbake dette samtykket fra innstillingene i spillet, noe som også er beskrevet i vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>personvernerklæring</u></color></link>.",
                footer = @"Ved å trykke på Fortsett nedenfor godtar du å være bundet av våre <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>bruksvilkår</u></b></color></link> og vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>personvernerklæring</u></b></color></link>, og du bekrefter at du er over 16 år.",
                toggle = "Jeg godtar",
                button = "Fortsette"
            },
            
            new GDPRTranslation()
            {
                lang = "sv",
                langType = SystemLanguage.Swedish,
                header = "Vi bryr oss om din integritet",
                policy = @"Härmed går jag med på att mina personuppgifter (inklusive enhetsinformation och mina inställningar) används och delas med <link=" + AD_PARTNERS_LINK + "><color=#{0}><u>reklamnätverk</u></color></link> i syfte att ge mig riktad reklam i spelet. Jag är medveten om att jag kan dra tillbaka mitt samtycke när som helst i spelets inställningar, enligt vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><u>integritetspolicy</u></color></link>.",
                footer = @"Genom att trycka på Fortsätt nedan går du med på att följa våra <link=" + TERMS_OF_USE_LINK + "><color=#{0}><b><u>användarvillkor</u></b></color></link> och vår <link=" + PRIVACY_POLICY_LINK + "><color=#{0}><b><u>integritetspolicy</u></b></color></link>, samt bekräftar att du har fyllt 16 år.",
                toggle = "Jag godkänner",
                button = "Fortsätta"
            }
        };
        
        public static GDPRTranslation DefaultTranslation => translations[0];

        public static List<GDPRTranslation> Translations => translations;
        
        public static GDPRTranslation GetTranslation( GDPRConfig config, string forceLang = "" )
        {
            if ( config == null || !config.IsTranslationEnabled )
            {
                return translations[0];
            }

            var forceLanguage = !string.IsNullOrEmpty( forceLang );
            
            // Check by Unity API.
            if ( Application.systemLanguage != SystemLanguage.Unknown && !forceLanguage )
            {
                for ( int i = 0; i < translations.Count; ++i )
                {
                    if ( Application.systemLanguage == translations[i].langType )
                    {
                        return translations[i];
                    }
                }
            }

            var lang = HNativeCountryCode.GetCountryCode().Language;
            var code = HNativeCountryCode.GetCountryCode().GetCountryCode();

            if ( forceLanguage )
            {
                lang = forceLang;
                code = forceLang;
            }
            
            // Check by full country code {xx-yy}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == code )
                {
                    return translations[i];
                }
            }
            
            // Check only via language code {xx}.
            for ( int i = 0; i < translations.Count; ++i )
            {
                if ( translations[i].lang == lang || translations[i].lang == code )
                {
                    return translations[i];
                }
            }

            return translations[0];
        }
    }
}