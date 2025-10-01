using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace RentalTourismSystem.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // Remover símbolos de moeda e espaços
            value = value.Replace("R$", "").Replace(" ", "").Trim();

            // Processar formato brasileiro: trocar vírgula por ponto
            if (value.Contains(","))
            {
                // Se tem ponto e vírgula, remover pontos (separadores de milhares)
                if (value.Contains("."))
                {
                    var lastCommaIndex = value.LastIndexOf(',');
                    var beforeComma = value.Substring(0, lastCommaIndex).Replace(".", "");
                    var afterComma = value.Substring(lastCommaIndex + 1);
                    value = $"{beforeComma}.{afterComma}";
                }
                else
                {
                    // Apenas vírgula, trocar por ponto
                    value = value.Replace(",", ".");
                }
            }

            // Tentar converter
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                    "Formato de número inválido. Use vírgula como separador decimal (ex: 80,50)");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }

    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(decimal) ||
                context.Metadata.ModelType == typeof(decimal?))
            {
                return new DecimalModelBinder();
            }

            return null;
        }
    }
}