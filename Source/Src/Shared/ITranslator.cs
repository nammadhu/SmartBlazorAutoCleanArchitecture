using SHARED.DTOs;

namespace SHARED;

public interface ITranslator
    {
    string this[string name]
        {
        get;
        }

    string GetString(string name);

    string GetString(TranslatorMessageDto input);
    }
