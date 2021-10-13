namespace UtilitiesLib
{
    public static class StringConverter
    {
        public static int ParseStringToInteger(string strNumber)
        {
            int defaultValue = 0;
            int parsedValue = defaultValue;

            if (int.TryParse(strNumber, out parsedValue))
                return parsedValue;

            return defaultValue;
        }

        public static double ParseStringToDouble(string strNumber)
        {
            double defaultValue = 0d;
            double parsedValue = defaultValue;

            if (double.TryParse(strNumber, out parsedValue))
                return parsedValue;

            return parsedValue;
        }

        public static int ParseStringToInteger(string strNumber, int lowerLimit, int upperLimit)
        {
            int defaultValue = 0;
            int parsedValue = defaultValue;

            if (int.TryParse(strNumber, out parsedValue))
                if ((parsedValue >= lowerLimit) && (parsedValue <= upperLimit))
                    return parsedValue;

            return defaultValue;
        }

        public static double ParseStringToDouble(string strNumber, double lowerLimit, double upperLimit)
        {
            double defaultValue = 0d;
            double parsedValue = defaultValue;

            if (double.TryParse(strNumber, out parsedValue))
                if ((parsedValue >= lowerLimit) && (parsedValue <= upperLimit))
                    return parsedValue;

            return defaultValue;
        }
    }
}
